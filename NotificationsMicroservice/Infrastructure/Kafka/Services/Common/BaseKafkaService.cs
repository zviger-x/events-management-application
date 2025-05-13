using Confluent.Kafka;
using Infrastructure.Kafka.MessageHandlers.Interfaces.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Services.Background;

namespace Infrastructure.Kafka.Services.Common
{
    public abstract class BaseKafkaService<TMessageHandler> : AdvancedBackgroundService
        where TMessageHandler : IKafkaMessageHandler
    {
        protected readonly TMessageHandler _messageHandler;
        protected readonly ILogger<BaseKafkaService<TMessageHandler>> _logger;
        protected readonly ConsumerConfig _kafkaConfig;

        private IConsumer<Ignore, string> _consumer;

        public BaseKafkaService(
            TMessageHandler kafkaMessageHandler,
            ILogger<BaseKafkaService<TMessageHandler>> logger,
            IOptions<KafkaServerConfig> kafkaConfig)
            : base(logger)
        {
            _logger = logger;

            _kafkaConfig = new ConsumerConfig
            {
                BootstrapServers = kafkaConfig.Value.BootstrapServers,
                GroupId = kafkaConfig.Value.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _messageHandler = kafkaMessageHandler;
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka service started. Service: {service}", GetType().Name);

            _consumer = new ConsumerBuilder<Ignore, string>(_kafkaConfig).Build();

            await WaitForTopicToBecomeAvailableAsync(_messageHandler.Topic, cancellationToken);

            // Ожидаем, чтобы не было ошибки при подписке
            // потому что операция может пройти через миллисекунду, после проверки
            // и "для подписки" топики будут ещё не готовы
            await Task.Delay(1000, cancellationToken);

            SubscribeToTopic(_consumer, _messageHandler.Topic);

            _logger.LogInformation("Kafka is ready to receive messages. Service: {service}", GetType().Name);
        }

        protected override async Task ExecuteIterationAsync(CancellationToken cancellationToken)
        {
            var consumeResult = default(ConsumeResult<Ignore, string>);

            try
            {
                consumeResult = _consumer.Consume(cancellationToken);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka ConsumeException. Topic: {topic}, Reason: {reason}", _messageHandler.Topic, ex.Error.Reason);
                return;
            }

            if (consumeResult?.Message?.Value == null)
                return;

            var topic = consumeResult.Topic;
            var message = consumeResult.Message.Value;

            _logger.LogInformation("Received from {topicPartitionOffset}: {message}", consumeResult.TopicPartitionOffset, message);

            try
            {
                await _messageHandler.HandleMessage(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while parsing json message: {message}", ex.Message);
            }
        }

        protected override async Task CleanupAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();

            _logger.LogInformation("Kafka service stopped. Service: {service}", GetType().Name);

            await Task.CompletedTask;
        }

        private async Task WaitForTopicToBecomeAvailableAsync(string topic, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking if topic is available. Topic: {topic}", topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                var config = new AdminClientConfig { BootstrapServers = _kafkaConfig.BootstrapServers };
                using var adminClient = new AdminClientBuilder(config).Build();

                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                var exists = metadata.Topics.Any(t => t.Topic == topic && t.Error.Code == ErrorCode.NoError);

                if (exists)
                {
                    _logger.LogInformation("Topic {topic} is now available", topic);
                    break;
                }

                _logger.LogWarning("Topic {topic} not yet available, retrying...", topic);

                await Task.Delay(5000, cancellationToken);
            }
        }

        private void SubscribeToTopic(IConsumer<Ignore, string> consumer, string topic)
        {
            _logger.LogInformation("Subscribe to topic {topic}...", topic);
            consumer.Subscribe(topic);
        }
    }
}
