using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.DAL.EF.Repositories;
using QConsoleWeb.DAL.EF.EDM;
using QConsoleWeb.DAL.EF.Interfaces;

namespace QConsoleWeb.DAL.EF.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<Logtable> LogtableRepository { get; }
        IRepository<Dictionaries> DictionariesRepository { get; }
        //LogtableRepository LogtableRepository { get; }
        /// 
        /// Commits all changes
        /// 
        void Save();
        /// 
        /// Discards all changes that has not been commited
        /// 
        void RejectChanges();
        void Dispose();
    }
}
