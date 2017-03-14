using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WS.DAL;

namespace WS.BLL
{
    public abstract class Base_BLL<T> where T : class, new()
    {
        protected Base_DAL<T> dal = new Base_DAL<T>();

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <param name="isSave">是否立即保存</param>
        /// <returns>受影响的记录数</returns>
        public int Add(T entity, bool isSave = true)
        {
            return dal.Add(entity, isSave);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <param name="isSave">是否立即保存</param>
        /// <returns>受影响的记录数</returns>
        public int Update(T entity, bool isSave = true)
        {
            return dal.Update(entity, isSave);
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="whereLambda">表达式</param>
        /// <param name="isSave">是否保存</param>
        /// <returns>受影响的记录数</returns>
        public int Delete(Expression<Func<T, bool>> whereLambda, bool isSave = true)
        {
            return dal.Delete(whereLambda, isSave);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns>受影响的记录数</returns>
        public int SaveChange()
        {
            return dal.SaveChange();
        }
        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="findLambda">查询表达式</param>
        /// <returns>数据实体</returns>
        public T Get(Expression<Func<T, bool>> findLambda)
        {
            return dal.Get(findLambda);
        }
        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="whereLandba">查询条件</param>
        /// <returns>数据列表</returns>

        public IQueryable<T> GetList(Expression<Func<T, bool>> whereLandba)
        {
            return dal.GetList(whereLandba);
        }
        public IQueryable<T> GetList()
        {
            return dal.GetList();
        }
        public int GetCount(Expression<Func<T, bool>> whereLandba)
        {
            return dal.GetCount(whereLandba);
        }


        /// <summary>
        /// 查询数据列表并排序
        /// </summary>
        /// <param name="whereLandba">查询条件</param>
        /// <param name="orderLandba">排序条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>数据列表</returns>
        public IQueryable<T> GetListOrderBy<TKey>(Expression<Func<T, bool>> whereLandba, Expression<Func<T, TKey>> orderLandba, bool isAsc = true)
        {
            return dal.GetListOrderBy(whereLandba, orderLandba, isAsc);

        }


        /// <summary>
        /// 查询数据列表并排序
        /// </summary>
        /// <param name="whereLandba">查询条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="orderLandbas">多个排序条件</param>

        /// <returns>数据列表</returns>
        public IQueryable<T> GetListOrderBys<TKey>(Expression<Func<T, bool>> whereLandba, bool isAsc, params Expression<Func<T, TKey>>[] orderLandbas)
        {
            return dal.GetListOrderBys(whereLandba, isAsc, orderLandbas);




        }



        /// <summary>
        /// 分页查询数据列表并排序
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="whereLandba">查询条件</param>
        /// <param name="orderLandba">排序条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>数据列表</returns>
        public IQueryable<T> GetPageListOrderBy<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLandba, Expression<Func<T, TKey>> orderLandba, bool isAsc = true)
        {

            return dal.GetPageListOrderBy(pageIndex, pageSize, whereLandba, orderLandba, isAsc);
        }


        /// <summary>
        /// 分页查询数据列表并排序
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="whereLandba">查询条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="orderLandbas">多个排序条件</param>
        /// <returns>数据列表</returns>
        public IQueryable<T> GetPageListOrderBys<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLandba, bool isAsc = true, params Expression<Func<T, TKey>>[] orderLandbas)
        {
            return dal.GetPageListOrderBys(pageIndex, pageSize, whereLandba, isAsc, orderLandbas);

        }
    }
}