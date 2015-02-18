using reexjungle.foundation.essentials.concretes;
using reexjungle.foundation.essentials.contracts;
using reexjungle.infrastructure.concretes.operations;
using reexjungle.infrastructure.contracts;
using reexjungle.infrastructure.io.concretes;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.interfaces.contracts.live;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reexjungle.xcal.service.interfaces.concretes.live
{
    public class AdminService : Service, IAdminService
    {
        private ILogFactory logfactory;
        private IAdminRepository repository;

        private ILog log = null;

        private ILog logger
        {
            get { return (log != null) ? this.log : this.logfactory.GetLogger(this.GetType()); }
        }

        public ILogFactory LogFactory
        {
            get { return this.logfactory; }
            set
            {
                if (value == null) throw new ArgumentNullException("Logger");
                this.logfactory = value;
                this.log = logfactory.GetLogger(this.GetType());
            }
        }

        public IAdminRepository AdminRepository
        {
            get { return this.repository; }
            set
            {
                if (value == null) throw new ArgumentNullException("AdminRepository");
                this.repository = value;
            }
        }

        public AdminService()
            : base()
        {
            this.AdminRepository = this.TryResolve<IAdminRepository>();
            this.LogFactory = this.TryResolve<ILogFactory>();
        }

        public AdminService(IAdminRepository repository, ILogFactory logger)
            : base()
        {
            this.AdminRepository = repository;
            this.LogFactory = logger;
        }

        public void Post(FlushDatabase request)
        {
            try
            {
                if (request.Mode != null && request.Mode.HasValue)
                    this.AdminRepository.Flush(request.Mode.Value);
                else
                    this.AdminRepository.Flush();
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }
    }
}