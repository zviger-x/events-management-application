namespace Application.UseCases.Interfaces
{
    public interface IUseCase<TResponse>
    {
        TResponse Execute();
    }

    public interface IUseCase<TRequest, TResponse>
    {
        TResponse Execute(TRequest request);
    }

    public interface IUseCase<TR1, TR2, TResponse>
    {
        TResponse Execute(TR1 requestParam1, TR2 requestParam2);
    }

    public interface IUseCase<TR1, TR2, TR3, TResponse>
    {
        TResponse Execute(TR1 requestParam1, TR2 requestParam2, TR3 requestParam3);
    }

    public interface IUseCase<TR1, TR2, TR3, TR4, TResponse>
    {
        TResponse Execute(TR1 requestParam1, TR2 requestParam2, TR3 requestParam3, TR4 requestParam4);
    }
}
