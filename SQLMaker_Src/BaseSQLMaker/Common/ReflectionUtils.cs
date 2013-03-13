using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Carpa.Common
{
    /// <summary>
    /// 代表 PropertyInfo 或者 FieldInfo
    /// </summary>
    public sealed class PropInfo
    {
        private object obj;
        private MemberInfo member;
        private PropertyInfo prop;
        private FieldInfo field;

        private PropInfo(object obj, MemberInfo member)
        {
            this.obj = obj;
            this.member = member;
        }

        internal PropInfo(object obj, PropertyInfo prop)
            : this(obj, (MemberInfo)prop)
        {
            this.prop = prop;
        }

        internal PropInfo(object obj, FieldInfo field)
            : this(obj, (MemberInfo)field)
        {
            this.field = field;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return member.Name; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            get
            {
                if (prop != null)
                {
                    return prop.GetValue(obj, null);
                }
                else if (field != null)
                {
                    return field.GetValue(obj);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            set
            {
                if (prop != null)
                {
                    prop.SetValue(obj, value, null);
                }
                else if (field != null)
                {
                    field.SetValue(obj, value);
                }
            }
        }

        /// <summary>
        /// 类
        /// </summary>
        public Type Type
        {
            get { return prop != null ? prop.PropertyType : field.FieldType; }
        }
    }

    /// <summary>
    /// 反射辅助类
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// 获取一个对象的属性，先找Property、再找Field，都找不到则返回null
        /// </summary>
        public static PropInfo FindProp(object obj, string name)
        {
            if (obj == null) return null;
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            Type type = obj.GetType();
            PropertyInfo prop = type.GetProperty(name, flags);
            if (prop != null)
            {
                return new PropInfo(obj, prop);
            }
            else
            {
                FieldInfo field = type.GetField(name, flags);
                if (field != null)
                {
                    return new PropInfo(obj, field);
                }
            }
            return null;
        }

        /// <summary>
        /// 如果 member 既不是 Property 也不是 Field 则返回 null
        /// </summary>
        public static PropInfo FindProp(object obj, MemberInfo member)
        {
            if (member is PropertyInfo)
            {
                return new PropInfo(obj, (PropertyInfo)member);
            }
            else if (member is FieldInfo)
            {
                return new PropInfo(obj, (FieldInfo)member);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取一个实例的所有属性和字段
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>属性数组</returns>
        public static PropInfo[] GetProps(object obj)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            Type type = obj.GetType();
            List<PropInfo> list = new List<PropInfo>();
            foreach (PropertyInfo propertyInfo in type.GetProperties(flags))
            {
                list.Add(new PropInfo(obj, (PropertyInfo)propertyInfo));
            }
            foreach (FieldInfo fieldInfo in type.GetFields(flags))
            {
                list.Add(new PropInfo(obj, (FieldInfo)fieldInfo));
            }
            PropInfo[] answer = new PropInfo[list.Count];
            list.CopyTo(answer);
            return answer;
        }

        /// <summary>
        /// 是否简单类型
        /// </summary>
        public static bool IsPrimitiveType(Type type)
        {
            return DoGetIsPrimitiveType(type);
        }

        /// <summary>
        /// 是否简单类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="underlyingType"></param>
        /// <returns></returns>
        internal static bool IsPrimitiveType(Type type, out Type underlyingType)
        {
            underlyingType = type;
            bool answer = DoGetIsPrimitiveType(type);
            if (answer)
            {
                return true;
            }
            if (type.IsValueType && type.IsGenericType)
            {
                underlyingType = Nullable.GetUnderlyingType(type);
                return DoGetIsPrimitiveType(underlyingType);
            }
            return false;
        }

        private static bool DoGetIsPrimitiveType(Type type)
        {
            bool answer = type.IsPrimitive || type == typeof(String) || type == typeof(DateTime)
                            || type == typeof(decimal);
            return answer;
        }

        /// <summary>
        /// 取属性，属性不存在则抛出异常
        /// </summary>
        public static PropInfo GetProp(object obj, string name)
        {
            PropInfo prop = FindProp(obj, name);
            if (prop == null)
            {
                throw new Exception(GetNoPropMessage(obj, name));
            }
            return prop;
        }

        /// <summary>
        /// 取属性值，属性不存在则抛出异常
        /// </summary>
        public static object GetPropValue(object obj, string name)
        {
            PropInfo prop = GetProp(obj, name);
            return prop.Value;
        }

        /// <summary>
        /// 设属性值，属性不存在则抛出异常
        /// </summary>
        public static void SetPropValue(object obj, string name, object value)
        {
            PropInfo prop = GetProp(obj, name);
            prop.Value = value;
        }

        /// <summary>
        /// 取属性不存在的提示信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetNoPropMessage(object obj, string name)
        {
            return GetNoPropMessage(obj.GetType(), name);
        }

        /// <summary>
        /// 取属性不存在的提示信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetNoPropMessage(Type type, string name)
        {
            return string.Format("“{0}.{1}”属性不存在。", type.Name, name);
        }

        /// <summary>
        /// 放回特定类型的子类型
        /// </summary>
        public static Type GetArrayElementType(Type type)
        { // 抄自 System.Xml.Serialization.TypeScope (internal)
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            if (typeof(ICollection).IsAssignableFrom(type)) // 泛型List有实现 ICollection
            {
                return GetCollectionElementType(type);
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return GetEnumeratorElementType(type);
            }
            return null;
        }

        // 抄自 System.Xml.Serialization.TypeScope
        private static Type GetCollectionElementType(Type type)
        {
            PropertyInfo indexer = GetDefaultIndexer(type);
            return indexer != null ? indexer.PropertyType : null;
        }

        // 抄自 System.Xml.Serialization.TypeScope
        private static PropertyInfo GetDefaultIndexer(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return null; // 不抛出异常
                // throw new NotSupportedException(string.Format("The type {0} is not supported because it implements IDictionary.", type.FullName));
            }
            MemberInfo[] defaultMembers = type.GetDefaultMembers();
            PropertyInfo info = null;
            if ((defaultMembers != null) && (defaultMembers.Length > 0))
            {
                for (Type type2 = type; type2 != null; type2 = type2.BaseType)
                {
                    for (int i = 0; i < defaultMembers.Length; i++)
                    {
                        if (defaultMembers[i] is PropertyInfo)
                        {
                            PropertyInfo info2 = (PropertyInfo)defaultMembers[i];
                            if ((info2.DeclaringType == type2) && info2.CanRead)
                            {
                                ParameterInfo[] parameters = info2.GetGetMethod().GetParameters();
                                if ((parameters.Length == 1) && (parameters[0].ParameterType == typeof(int)))
                                {
                                    info = info2;
                                    break;
                                }
                            }
                        }
                    }
                    if (info != null)
                    {
                        break;
                    }
                }
            }
            if (info == null)
            {
                throw new InvalidOperationException(string.Format("You must implement a default accessor on {0} because it inherits from ICollection.", type.FullName));
            }
            return info;
        }

        // 抄自 System.Xml.Serialization.TypeScope
        private static Type GetEnumeratorElementType(Type type)
        {
            if (type.IsGenericType && type.GetGenericArguments().Length == 1)
            {
                return type.GetGenericArguments()[0];
            }
            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                return null;
            }
            MethodInfo method = type.GetMethod("GetEnumerator", new Type[0]);
            if ((method == null) || !typeof(IEnumerator).IsAssignableFrom(method.ReturnType))
            {
                method = null;
                foreach (MemberInfo info2 in type.GetMember("System.Collections.Generic.IEnumerable<*", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    method = info2 as MethodInfo;
                    if ((method != null) && typeof(IEnumerator).IsAssignableFrom(method.ReturnType))
                    {
                        //flags |= TypeFlags.GenericInterface;
                        break;
                    }
                    method = null;
                }
                if (method == null)
                {
                    //flags |= TypeFlags.UsePrivateImplementation;
                    method = type.GetMethod("System.Collections.IEnumerable.GetEnumerator", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
                }
            }
            if ((method == null) || !typeof(IEnumerator).IsAssignableFrom(method.ReturnType))
            {
                return null;
            }
            PropertyInfo property = method.ReturnType.GetProperty("Current");
            Type type2 = (property == null) ? typeof(object) : property.PropertyType;
            MethodInfo info4 = type.GetMethod("Add", new Type[] { type2 });
            if ((info4 == null) && (type2 != typeof(object)))
            {
                type2 = typeof(object);
            }
            return type2;
        }
    }
}
