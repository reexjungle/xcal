using reexjungle.xcal.service.interfaces.contracts.live;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using System;
using ServiceStack.ServiceHost;

namespace reexjungle.xcal.service.interfaces.concretes.live
{
    /// <summary>
    ///
    /// </summary>
    public class AdminService : Service, IAdminService
    {
        private readonly ILogFactory factory;
        private readonly IAdminRepository repository;

        private ILog log;

        private ILog logger
        {
            get
            {
                return log ?? (log = factory.GetLogger(GetType()));
            }
        }

        public ILogFactory LogFactory
        {
            get { return factory; }
        }

        public IAdminRepository AdminRepository
        {
            get { return repository; }
        }

        public AdminService(IAdminRepository repository, ILogFactory factory)
        {
            this.repository = repository;
            this.factory = factory;
        }

        public void Post(FlushDatabase request)
        {
            try
            {
                if (request.Force != null)
                {
                    AdminRepository.Flush(request.Force.Value);
                }
                else
                {
                    AdminRepository.Flush();
                }

                //invalidate cache
                Cache.FlushAll();
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (ApplicationException ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString()); throw;
            }
        }
    }
}