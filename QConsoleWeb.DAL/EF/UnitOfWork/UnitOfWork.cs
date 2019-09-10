using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QConsoleWeb.DAL.EF.EDM;
using QConsoleWeb.DAL.EF.Interfaces;
using QConsoleWeb.DAL.EF.Repositories;

namespace QConsoleWeb.DAL.EF.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BaseEntities _dbContext;

        #region Repositories

        private IRepository<Logtable> logtableRepository;
        public IRepository<Logtable> LogtableRepository
        {
            get
            {
                if (logtableRepository == null)
                    logtableRepository = new GenericRepository<Logtable>(_dbContext);
                return logtableRepository;
            }
        }

        private IRepository<Dictionaries> dictionariesRepository;
        public IRepository<Dictionaries> DictionariesRepository
        {
            get
            {
                if (dictionariesRepository == null)
                    dictionariesRepository = new GenericRepository<Dictionaries>(_dbContext);
                return dictionariesRepository;
            }
        }
        #endregion

        public UnitOfWork(string conn)
        {
            _dbContext = new BaseEntities(conn);

            //################# DEBUGGINGGGG ###############
            //_dbContext.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            //################# DEBUGGINGGGG ###############
        }

        public void RejectChanges()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries()
                  .Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }

        //public void Save()
        //{
        //    _dbContext.SaveChanges();
        //}

        public void Save()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
            //                ve.PropertyName,
            //                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
            //                ve.ErrorMessage);
            //        }
            //    }
            //    throw;
            //}
            catch
            {
                throw;
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
