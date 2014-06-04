using System;
using System.Linq;
using System.Collections.Generic;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.infrastructure.operations.contracts;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.xcal.service.interfaces.contracts.live;
using reexmonkey.infrastructure.io.concretes;
using reexmonkey.infrastructure.operations.concretes;
using reexmonkey.foundation.essentials.contracts;

namespace reexmonkey.xcal.service.interfaces.concretes.live
{
    public class AdminService: Service, IAdminService
    {      
        private ILogFactory logfactory;
        private IAdminRepository repository;

        private ILog log = null;
        private ILog logger
        {
            get { return (log != null)? this.log: this.logfactory.GetLogger(this.GetType()); }
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

        public AdminService() : base() 
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
                if(request.Hard != null && request.Hard.HasValue) 
                    this.AdminRepository.Flush(request.Hard.Value);
                else 
                    this.AdminRepository.Flush();
            }
            catch (InvalidOperationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (ApplicationException ex) { this.logger.Error(ex.ToString()); throw; }
            catch (Exception ex) { this.logger.Error(ex.ToString()); throw; }
        }
    }
}
