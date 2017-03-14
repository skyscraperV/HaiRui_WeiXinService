using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WS.Model;

namespace WS.DAL
{
    public class Base_DAL<T> where T : class, new()
    {
        /// <summary>
        /// EF上下文对象
        /// </summary>
        WSDBEntities db = new WSDBEntities();
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <param name="isSave">是否立即保存</param>
        /// <returns>受影响的记录数</returns>
        public int Add(T entity, bool isSave = true)
        {
            db.Set<T>().Add(entity);
            return isSave ? db.SaveChanges() : 0;
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <param name="isSave">是否立即保存</param>
        /// <returns>受影响的记录数</returns>
        public int Update(T entity, bool isSave = true)
        {
            db.Set<T>().Attach(entity);
            db.Entry<T>(entity).State = EntityState.Modified;
            return isSave ? db.SaveChanges() : 0;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="whereLambda">表达式</param>
        /// <param name="isSave">是否保存</param>
        /// <returns>受影响的记录数</returns>
        public int Delete(Expression<Func<T, bool>> whereLambda, bool isSave = true)
        {
            //3.1查询要删除的数据
            List<T> listDeleting = db.Set<T>().Where(whereLambda).ToList();
            //3.2将要删除的数据 用删除方法添加到 EF 容器中
            listDeleting.ForEach(u =>
            {
                db.Set<T>().Attach(u);//先附加到 EF容器
                db.Set<T>().Remove(u);//标识为 删除 状态
            });
            //3.3一次性 生成sql语句到数据库执行删除
            return isSave ? db.SaveChanges() : 0;
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns>受影响的记录数</returns>
        public int SaveChange()
        {
            return db.SaveChanges();
        }
        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="findLambda">查询表达式</param>
        /// <returns>数据实体</returns>
        public T Get(Expression<Func<T, bool>> findLambda)
        {
            return db.Set<T>().Where(findLambda).FirstOrDefault();
        }
        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="whereLandba">查询条件</param>
        /// <returns>数据列表</returns>

        public IQueryable<T> GetList(Expression<Func<T, bool>> whereLandba)
        {
            IQueryable<T> _tIQueryable = db.Set<T>().Where(whereLandba);
            return _tIQueryable;
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
            if (isAsc)
            {
                return db.Set<T>().Where(whereLandba).OrderBy(orderLandba);

            }
            else
            {

                return db.Set<T>().Where(whereLandba).OrderByDescending(orderLandba);

            }


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

            IQueryable<T> list = db.Set<T>().Where(whereLandba);


            if (isAsc)
            {

                foreach (var orderLandba in orderLandbas)
                {
                    list = list.OrderBy(orderLandba);
                }
            }
            else
            {
                foreach (var orderLandba in orderLandbas)
                {
                    list = list.OrderByDescending(orderLandba);
                }
            }

            return list;




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

            if (isAsc)
            {
                return db.Set<T>().Where(whereLandba).OrderBy(orderLandba).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            }
            else
            {
                return db.Set<T>().Where(whereLandba).OrderByDescending(orderLandba).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            }

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
            IQueryable<T> list = db.Set<T>().Where(whereLandba);

            if (isAsc)
            {
                foreach (var orderLandba in orderLandbas)
                {
                    list = list.OrderBy(orderLandba);
                }
            }
            else
            {
                foreach (var orderLandba in orderLandbas)
                {
                    list = list.OrderByDescending(orderLandba);
                }
            }

            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return list;

        }
        /// <summary>
        /// 查询所有数据列表
        /// </summary>
        /// <returns>数据列表</returns>

        public IQueryable<T> GetList()
        {
            IQueryable<T> _tIQueryable = db.Set<T>();
            return _tIQueryable;
        }
        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="whereLandba">查询表达式</param>
        /// <returns>数据数量</returns>
        public int GetCount(Expression<Func<T, bool>> whereLandba)
        {
            int count = db.Set<T>().Count(whereLandba);
            return count;
        }
    }
}
