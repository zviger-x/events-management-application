namespace Infrastructure.Common
{
    internal static class NotificationMessageFactory
    {
        public static string EventCompleted(string eventName, DateTime completedAt)
            => $"Мероприятие \"{eventName}\" завершилось {completedAt:dd.MM.yyyy в HH:mm}. " +
               "Не забудьте оставить свой отзыв или комментарий. Спасибо, что были с нами!";

        public static string EventUpcoming(string eventName, DateTime startTime)
            => $"Напоминаем: завтра состоится мероприятие \"{eventName}\" ({startTime:dd.MM.yyyy в HH:mm}). Не пропустите!";

        public static string EventUpdated(string eventName, DateTime updatedAt)
            => $"Событие \"{eventName}\" было обновлено {updatedAt:dd.MM.yyyy в HH:mm}. Пожалуйста, ознакомьтесь с актуальной информацией.";

        public static string PaymentConfirmed(string eventName, DateTime confirmedAt, float amount)
            => $"Оплата за участие в мероприятии \"{eventName}\" была успешно подтверждена " +
               $"{confirmedAt:dd.MM.yyyy в HH:mm}. Сумма: {amount:0.00}. Спасибо за вашу регистрацию!";
    }
}
