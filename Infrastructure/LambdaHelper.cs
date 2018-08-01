using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    class LambdaHelper
    {
        public enum CompareType
        {
            Equal = 1,
            GreaterThan = 2,
            GreaterThanOrEqual = 3,
            LessThan = 4,
            LessThanOrEqual = 5,
            Contains = 6,
            UnContains = 7,
            StartsWith = 8,
            EndsWith = 9
        }

        /// <summary>
        /// 创建lambda表达式：p=>true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return p => true;
        }

        /// <summary>
        /// 创建lambda表达式：p=>false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return p => false;
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static Expression<Func<T, TKey>> GetOrderExpression<T, TKey>(string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            return Expression.Lambda<Func<T, TKey>>(Expression.Property(parameter, propertyName), parameter);
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName == propertyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateEqual<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");//创建参数p
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(propertyValue);//创建常数
            return Expression.Lambda<Func<T, bool>>(Expression.Equal(member, constant), parameter);
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName != propertyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateNotEqual<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");//创建参数p
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(propertyValue);//创建常数
            return Expression.Lambda<Func<T, bool>>(Expression.NotEqual(member, constant), parameter);
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName > propertyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateGreaterThan<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");//创建参数p
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(propertyValue);//创建常数
            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(member, constant), parameter);
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName < propertyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateLessThan<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");//创建参数p
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(propertyValue);//创建常数
            return Expression.Lambda<Func<T, bool>>(Expression.LessThan(member, constant), parameter);
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName >= propertyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateGreaterThanOrEqual<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");//创建参数p
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(propertyValue);//创建常数
            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(member, constant), parameter);
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName <= propertyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateLessThanOrEqual<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");//创建参数p
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(propertyValue);//创建常数
            return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(member, constant), parameter);
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName.Contains(propertyValue)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> GetContains<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            ConstantExpression constant = Expression.Constant(propertyValue, typeof(string));
            return Expression.Lambda<Func<T, bool>>(Expression.Call(member, method, constant), parameter);
        }

        /// <summary>
        /// 创建lambda表达式：!(p=>p.propertyName.Contains(propertyValue))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> GetNotContains<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            ConstantExpression constant = Expression.Constant(propertyValue, typeof(string));
            return Expression.Lambda<Func<T, bool>>(Expression.Not(Expression.Call(member, method, constant)), parameter);
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName.StartsWith(propertyValue)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> GetStartsWith<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            MethodInfo method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            ConstantExpression constant = Expression.Constant(propertyValue, typeof(string));
            return Expression.Lambda<Func<T, bool>>(Expression.Call(member, method, constant), parameter);
        }
        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName.EndsWith(propertyValue)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> GetEndsWith<T>(string propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            MethodInfo method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
            ConstantExpression constant = Expression.Constant(propertyValue, typeof(string));
            return Expression.Lambda<Func<T, bool>>(Expression.Call(member, method, constant), parameter);
        }

        public static Expression GetExpressionBody<T>(CompareType compareType, ParameterExpression parameter, string propertyOrFieldName, object value)
        {
            var type = typeof(T);
            object val = default(T);
            Type dyType = GetValue(type, propertyOrFieldName, value, out val);
            if (val == null) return null;
            MemberExpression member = Expression.PropertyOrField(parameter, propertyOrFieldName);
            ConstantExpression constant = Expression.Constant(val, dyType);
            Expression expr = null;
            MethodInfo method = null;
            switch (compareType)
            {
                case CompareType.Equal:
                    expr = Expression.Equal(member, constant);
                    break;
                case CompareType.GreaterThan:
                    expr = Expression.GreaterThan(member, constant);
                    break;
                case CompareType.GreaterThanOrEqual:
                    expr = Expression.GreaterThanOrEqual(member, constant);
                    break;
                case CompareType.LessThan:
                    expr = Expression.LessThan(member, constant);
                    break;
                case CompareType.LessThanOrEqual:
                    expr = Expression.LessThanOrEqual(member, constant);
                    break;
                case CompareType.Contains:
                    method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    expr = Expression.Call(member, method, constant);
                    break;
                case CompareType.UnContains:
                    method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    expr = Expression.Not(Expression.Call(member, method, constant));
                    break;
                case CompareType.StartsWith:
                    method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                    expr = Expression.Call(member, method, constant);
                    break;
                case CompareType.EndsWith:
                    method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                    expr = Expression.Call(member, method, constant);
                    break;
                default:
                    expr = Expression.Equal(member, constant);
                    break;
            }
            return expr;
        }


        /// <summary>
        /// 根据实体类动态转换值类型,并输出转换后的值
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="propName">属性名</param>
        /// <param name="value">原始值</param>
        /// <param name="oVal">输出值</param>
        /// <returns>该属性对应的数据类型</returns>
        public static Type GetValue(Type type, string propName, object value, out object oVal)
        {
            Type objType = null;
            object objValue = null;
            foreach (var pi in type.GetProperties())
            {
                if (pi.Name == propName)
                {
                    objType = pi.PropertyType;
                    string typeStr = string.Empty;
                    bool isNullType = IsNullableType(pi.PropertyType);
                    if (isNullType)
                    {
                        typeStr = pi.PropertyType.GetGenericArguments()[0].Name.ToLower();
                    }
                    else
                    {
                        typeStr = pi.PropertyType.Name.ToLower();
                    }
                    switch (typeStr)
                    {
                        case "string":
                            objValue = value + "";
                            break;
                        case "datetime":
                            DateTime tempDateTime;
                            bool isDateTime = DateTime.TryParse(value + "", out tempDateTime);
                            if (isDateTime)
                            {
                                objValue = tempDateTime;
                            }
                            else
                            {
                                objValue = null;
                            }
                            break;
                        case "int16":
                        case "int32":
                        case "int64":
                            int tempint = 0;
                            bool isInt = int.TryParse(value + "", out tempint);
                            if (isInt)
                            {
                                objValue = tempint;
                            }
                            else
                            {
                                objValue = null;
                            }
                            break;
                        case "double":
                            double tempDouble = 0;
                            var tempStrDouble = value + "";
                            bool isDouble = double.TryParse(tempStrDouble, out tempDouble);
                            if (isDouble)
                            {
                                objValue = tempDouble;
                            }
                            else
                            {
                                objValue = null;
                            }
                            break;
                        case "decimal":
                            decimal tempDecimal = 0;
                            var tempStr = value + "";
                            bool isdecimal = decimal.TryParse(tempStr, out tempDecimal);
                            if (isdecimal)
                            {
                                objValue = tempDecimal;
                            }
                            else
                            {
                                objValue = null;
                            }
                            break;
                        default:

                            break;
                    }
                }
            }
            oVal = objValue;
            return objType;
        }

        /// <summary>
        /// 判定是否为可空类型
        /// </summary>
        /// <param name="theType">类型</param>
        /// <returns></returns>
        public static bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
