using System.Threading.Tasks;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Model.Common.GlobalEnums;

namespace BusinessLayer.Model.Interfaces
{
    public interface IBaseService<TEntity, TInfo>
        where TEntity : class, new()
        where TInfo : class, new()
    {
        string ServiceName { get; set; }
        TransactionStateEnum TransactionState { get; set; }
        TransactionStateEnum LastTransactionState { get; set; }
        TInfo DataInfo { get; set; }
        IList<TInfo> DataListInfo { get; set; }

        Task<bool> FindByCode(string code, bool onlyCheckIfExists = false);
        Task<bool> FindByFilter(Expression<Func<TEntity, bool>> filter, bool onlyCheckIfExists = false);
        Task<bool> GetAllByCompanyCode(string companyCode);
        Task<bool> GetAll(Expression<Func<TEntity, bool>> filter = null,
                               Expression<Func<TEntity, TEntity>> selector = null,
                               int currentPage = -1, int pageSize = -1,
                               Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
        Task<long> GetCount(Expression<Func<TEntity, bool>> filter = null);

        Task<bool> AddOrEdit(TInfo info);
        Task<bool> Add(TInfo info);
        Task<bool> Edit(TInfo info);

        Task<bool> OnBeforeSave();
        Task<bool> Save();
        Task<bool> OnAfterSave();

        Task<bool> OnBeforeInsert();
        Task<bool> Insert();
        Task<bool> OnAfterInsert();

        Task<bool> OnBeforeUpdate();
        Task<bool> Update();
        Task<bool> OnAfterUpdate();

        Task<bool> OnBeforeDelete();
        Task<bool> Delete(string code);
        Task<bool> OnAfterDelete();
        Task<bool> DeleteByFilter(Expression<Func<TEntity, bool>> filter);

        Task<bool> DirectDelete(string code);
        Task<bool> DirectDeleteByFilter(Expression<Func<TEntity, bool>> filter);
    }
}
