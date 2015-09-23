namespace reexjungle.xcal.service.repositories.contracts
{
    public interface IAdminRepository
    {
        void Flush(bool force = false);
    }
}