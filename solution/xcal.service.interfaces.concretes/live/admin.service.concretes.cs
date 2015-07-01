using reexjungle.xcal.service.interfaces.contracts.live;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using System;

namespace reexjungle.xcal.service.interfaces.concretes.live
{
    public class AdminService : Service, IAdminService
    {
        private ILogFactory logfactory;
        private IAdminRepository repository;

        private ILog log = null;

        private ILog logger
        {
            get { return (log != null) ? log : logfactory.GetLogger(GetType()); }
        }

        public ILogFactory LogFactory
        {
            get { return logfactory; }
            set
            {
                if (value == null) throw new ArgumentNullException("Logger");
                logfactory = value;
                log = logfactory.GetLogger(GetType());
            }
        }

        public IAdminRepository AdminRepository
        {
            get { return repository; }
            set
            {
                if (value == null) throw new ArgumentNullException("AdminRepository");
                repository = value;
            }
        }

        public AdminService()
            : base()
        {
            AdminRepository = TryResolve<IAdminRepository>();
            LogFactory = TryResolve<ILogFactory>();
        }

        public AdminService(IAdminRepository repository, ILogFactory logger)
            : base()
        {
            AdminRepository = repository;
            LogFactory = logger;
        }

        public void Post(FlushDatabase request)
        {
            try
            {
                if (request.Mode != null && request.Mode.HasValue)
                    AdminRepository.Flush(request.Mode.Value);
                else
                    AdminRepository.Flush();
            }
            catch (InvalidOperationException ex) { logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { logger.Error(ex.ToString()); throw; }
        }
    }
}