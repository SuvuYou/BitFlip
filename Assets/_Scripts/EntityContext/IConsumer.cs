public interface IConsumer<TContext>
{
    void Inject(TContext context);
}
