
using AutoMapper;
using BusinessLayer.Model.Common;
using BusinessLayer.Model.Common.GlobalEnums;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public abstract class BaseService<TEntity, TInfo>
        where TEntity : DataEntity, new()
        where TInfo : BaseInfo, new()
    {
        private readonly IRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public BaseService(IRepository<TEntity> repository,
            IMapper mapper,
            ILogger logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;

            Initialize();
        }

        private const string CONST_RECORD_NOT_FOUND = "Record not found";
        private const string CONST_ERR_DATA_STATE = "Cannot process with current Data State.";

        private Expression<Func<TEntity, bool>> _defaultFilterExpression;
        private string _tableName;

        public string ServiceName { get; set; }
        public TransactionStateEnum TransactionState { get; set; }
        public TransactionStateEnum LastTransactionState { get; set; }
        public TInfo DataInfo { get; set; }
        public IList<TInfo> DataListInfo { get; set; }    
        
        private void Initialize()
        {
            TransactionState = TransactionStateEnum.Empty;
            LastTransactionState = TransactionStateEnum.Empty;

            this.ServiceName = this.GetType().Name; ;

            BuildDefaultFilterExpression();
        }

        public async virtual Task<bool> FindByCode(string code, bool onlyCheckIfExists = false)
        {
            if (string.IsNullOrEmpty(code)) return false;
            return await FindByFilter(x => x.Code == code, onlyCheckIfExists);
        }

        public async virtual Task<bool> FindByFilter(Expression<Func<TEntity, bool>> filter, 
            bool onlyCheckIfExists = false)
        {
            bool found = false;

            try
            {
                //using (var uow = _repository.Create())
                //{
                //    var entity = await uow.GetEntityRepository<TEntity>().FindAsync(filter);

                //    if (entity == null)
                //    {
                //        if (!onlyCheckIfExists) DataInfo = null;
                //        return false;
                //    }

                //    if (onlyCheckIfExists) DataInfo = _mapper.Map<TInfo>(entity);
                //}


                filter = BuildFilterExpression(filter);
                var entity = await _repository.FindAsync(filter);

                if (entity == null)
                {
                    if (!onlyCheckIfExists) DataInfo = null;
                    return false;
                }

                if (!onlyCheckIfExists) DataInfo = _mapper.Map<TInfo>(entity);

                found = true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }

            return found;
        }

        public async virtual Task<bool> GetAllByCompanyCode(string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode)) return false;
            return await GetAll(x => x.CompanyCode == companyCode);
        }

        public async virtual Task<bool> GetAll(Expression<Func<TEntity, bool>> filter = null,
            Expression<Func<TEntity, TEntity>> selector = null,
            int currentPage = -1, int pageSize = -1,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            bool success = false;
            try
            {

                //using (var uow = _repository.Create())
                //{
                //    int skip = currentPage > 0 ? (currentPage - 1) * pageSize : 0;
                //    int take = pageSize > 0 ? pageSize : int.MaxValue;

                //    var pagedItems = await uow.GetEntityRepository<TEntity>()
                //       .GetAllAsync(filter, selector, skip, take, orderBy);

                //    DataListInfo = pagedItems.Select(x => _mapper.Map<TInfo>(x)).ToList();
                //}
                filter = BuildFilterExpression(filter);
                var pagedItems = await _repository.GetAllAsync(filter, selector,
                    pageSize,
                    currentPage > -1 ? (currentPage - 1) * pageSize : currentPage,
                    orderBy);

                DataListInfo = pagedItems.Select(x => _mapper.Map<TInfo>(x)).ToList();

                success = true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }

            return success;
        }

        public async virtual Task<long> GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            long count = 0;

            try
            {
                //using (var uow = _repository.Create())
                //{
                //    count = await uow.GetEntityRepository<TEntity>()
                //       .CountAsync(filter);
                //}
                filter = BuildFilterExpression(filter);
                count = await _repository.CountAsync(filter);

            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }

            return count;
        }
        
        public async virtual Task<bool> AddOrEdit(TInfo info)
        {
            bool success = false;

            await (string.IsNullOrEmpty(info.Code) ? Add(info) : Edit(info));

            success = await Save();

            return success;
        }
        public async virtual Task<bool> Add(TInfo info)
        {
            bool success = false;

            DataInfo = info;

            success = true;
            return success;
        }
        public async virtual Task<bool> Edit(TInfo info)
        {
            bool success = false;

            if (!(await FindByCode(info.Code)))
                throw new ApplicationException(CONST_RECORD_NOT_FOUND);

            DataInfo = info;

            success = true;
            return success;
        }
        public async virtual Task<bool> OnBeforeSave()
        {
            bool success = false;

            if (DataInfo == null) 
                throw new ApplicationException(CONST_ERR_DATA_STATE);

            success = true;
            return success;
        }
        public async virtual Task<bool> Save()
        {
            bool success = false;

            try
            {
                if (!await OnBeforeSave()) return false;

                await (string.IsNullOrEmpty(DataInfo.Code) ? Insert() : Update());

                success = true;

                await OnAfterSave();
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }

            return success;
        }
        public async virtual Task<bool> OnAfterSave()
        {
            bool success = false;

            success = true;
            return success;
        }
        public async virtual Task<bool> OnBeforeInsert()
        {
            bool success = false;

            DataInfo.Code = GenerateCode();
            if (!IsValidDataState())
                throw new ApplicationException(CONST_ERR_DATA_STATE);

            DataInfo.LastModified = DateTime.Now;

            TransactionState = TransactionStateEnum.Add;

            success = true;
            return success;
        }
        public async virtual Task<bool> Insert()
        {
            bool success = false;

            try
            {
                if (!await OnBeforeInsert()) return false;

                //using (var uow = _repository.Create())
                //{
                //    if (!await uow.GetEntityRepository<TEntity>().InsertAsync(_mapper.Map<TEntity>(DataInfo)))
                //        throw new ApplicationException("An error occured during insert.");
                //}
                DataInfo.SiteId = _tableName;
                if (!await _repository.InsertAsync(_mapper.Map<TEntity>(DataInfo)))
                    throw new ApplicationException("An error occured during insert.");

                success = true;

                await OnAfterInsert();
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }

            return success;
        }
        public async virtual Task<bool> OnAfterInsert()
        {
            bool success = false;

            LastTransactionState = TransactionStateEnum.Add;
            TransactionState = TransactionStateEnum.Empty;

            success = true;
            return success;
        }
        public async virtual Task<bool> OnBeforeUpdate()
        {
            bool success = false;

            if (!IsValidDataState())
                throw new ApplicationException(CONST_ERR_DATA_STATE);

            DataInfo.LastModified = DateTime.Now;

            TransactionState = TransactionStateEnum.Edit;

            success = true;
            return success;
        }
        public async virtual Task<bool> Update()
        {
            bool success = false;

            try
            {
                if (!await OnBeforeUpdate()) return false;

                //using (var uow = _repository.Create())
                //{
                //    if (!await uow.GetEntityRepository<TEntity>().UpdateAsync(_mapper.Map<TEntity>(DataInfo)))
                //        throw new ApplicationException("An error occured during update.");
                //}
                DataInfo.SiteId = _tableName;
                if (!await _repository.UpdateAsync(_mapper.Map<TEntity>(DataInfo)))
                    throw new ApplicationException("An error occured during update.");

                success = true;

                await OnAfterUpdate();
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }

            return success;
        }
        public async virtual Task<bool> OnAfterUpdate()
        {
            bool success = false;

            LastTransactionState = TransactionStateEnum.Edit;
            TransactionState = TransactionStateEnum.Empty;

            success = true;
            return success;
        }
        
        public async virtual Task<bool> OnBeforeDelete()
        {
            bool success = false;

            TransactionState = TransactionStateEnum.Delete;

            success = true;
            return success;
        }

        public async virtual Task<bool> Delete(string code)
        {
            bool success = false;
            
            try
            {
                if (!await OnBeforeDelete()) return false;

                //using (var uow = _repository.Create())
                //{
                //    if (!await uow.GetEntityRepository<TEntity>().DeleteAsync(x => x.Code == code))
                //        throw new ApplicationException("An error occured during delete.");
                //}

                if (!await _repository.DeleteAsync(x => x.Code == code))
                    throw new ApplicationException("An error occured during delete.");

                success = true;

                await OnAfterDelete();
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }

            return success;
        }

        public async virtual Task<bool> OnAfterDelete()
        {
            bool success = false;

            LastTransactionState = TransactionStateEnum.Delete;
            TransactionState = TransactionStateEnum.Empty;

            success = true;
            return success;
        }

        public async virtual Task<bool> DeleteByFilter(Expression<Func<TEntity, bool>> filter)
        {
            bool success = false;

            var selector = CreateCodeSelector();

            filter = BuildFilterExpression(filter);
            int pageSize = 50;
            bool done = false;
            while (!done)
            {
                await GetAll(filter: filter, 
                    selector: selector,  
                    currentPage: 1, 
                    pageSize: pageSize);

                foreach (var item in DataListInfo)
                {
                    await Delete(item.Code);
                }

                done = DataListInfo.Count() < pageSize;
            }

            success = true;
            return success;
        }

        public async virtual Task<bool> DirectDelete(string code)
        {
            return await DirectDeleteByFilter(x => x.Code == code);
        }

        public async virtual Task<bool> DirectDeleteByFilter(Expression<Func<TEntity, bool>> filter)
        {
            bool success = false;

            try
            {
                //using (var uow = _repository.Create())
                //{
                //    if (!await uow.GetEntityRepository<TEntity>().DeleteAsync(filter))
                //        throw new ApplicationException("An error occured during delete.");
                //}
                filter = BuildFilterExpression(filter);
                if (!await _repository.DeleteAsync(filter))
                    throw new ApplicationException("An error occured during delete.");

                success = true;

            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }

            return success;
        }

        private Expression<Func<TEntity, TEntity>> CreateCodeSelector()
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var codeBinding = Expression.Bind(typeof(TEntity).GetProperty("Code"), Expression.Property(parameter, "Code"));
            var body = Expression.MemberInit(Expression.New(typeof(TEntity)), codeBinding);

            return Expression.Lambda<Func<TEntity, TEntity>>(body, parameter);
        }

        private bool IsValidDataState()
        {
            return !(DataInfo == null || string.IsNullOrEmpty(DataInfo.Code));
        }

        private string GenerateCode()
        {
            return "{" + Guid.NewGuid().ToString() + "}";
        }

        protected void LogError(Exception ex)
        {
            _logger.Error($"Error in {ServiceName} : {ex.Message }");
        }

        private Expression<Func<TEntity, bool>> BuildFilterExpression(Expression<Func<TEntity, bool>> filter)
        {
            return filter == null ? this._defaultFilterExpression
                    : PredicateBuilder.And(filter, this._defaultFilterExpression);
        }

        private void BuildDefaultFilterExpression()
        {
            CreateTableName();
            _defaultFilterExpression = x => x.SiteId == _tableName;
        }

        private void CreateTableName()
        {
            _tableName = typeof(TEntity).Name;
        }
    }
}
