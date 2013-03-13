using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Carpa.Common;

namespace Carpa.Web.Script
{
    /// <summary>
    /// 哈希对象
    /// </summary>
    public interface IHashObject : IDictionary<string, object>
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        IHashObject Clone();

        /// <summary>
        /// 添加多个项
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        void Add(string[] keys, params object[] values);
        /// <summary>
        /// 拷贝所有数据到另一个实例
        /// </summary>
        /// <param name="dict"></param>
        void CopyTo(IDictionary<string, object> dict);

        /// <summary>
        /// 取指定的值，默认值为null或0或false或空字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetValue<T>(string key);

        /// <summary>
        /// 取指定的值，找不到则返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        T GetValue<T>(string key, T defaultValue);

        /// <summary>
        /// key存在则保持已有值不变返回false；不存在则设置并返回 true
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CheckSetValue(string key, object value);
    }

    /// <summary>
    /// 哈希对象列表
    /// </summary>
    public interface IHashObjectList : IList<IHashObject>
    {
        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        void Add(string[] keys, params object[] values);

        /// <summary>
        /// 取一个范围，参数不对会抛出异常
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IHashObjectList GetRange(int index, int count);

        /// <summary>
        /// 按某个字段排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ascending">是否升序</param>
        void Sort(string key, bool ascending);

        /// <summary>
        /// 建索引快速查找
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IHashObject Find(string key, object value);
    }

    /// <summary>
    /// 哈希对象
    /// </summary>
    [Serializable]
    public class HashObject : Dictionary<string, object>, IHashObject
    {
        /// <summary>
        /// 默认
        /// </summary>
        public HashObject()
            : base()
        {
        }

        /// <summary>
        /// 从已有数据复制
        /// </summary>
        /// <param name="dictionary"></param>
        public HashObject(IDictionary<string, object> dictionary)
            : base(dictionary)
        {
        }


        /// <summary>
        /// 传入许多键和值
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        public HashObject(string[] keys, params object[] values)
        {
            InternalAdd(this, keys, values);
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public IHashObject Clone()
        {
            HashObject clone = new HashObject();
            foreach (KeyValuePair<string, object> pair in this)
            {
                clone.Add(pair.Key, pair.Value);
            }
            return clone;
        }

        /// <summary>
        /// 添加条目
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        public void Add(string[] keys, params object[] values)
        {
            InternalAdd(this, keys, values);
        }

        /// <summary>
        /// 存取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new object this[string key]
        {
            get
            {
                try
                {
                    return base[key];
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException(string.Format("关键字“{0}”不在HashObject中。", key));
                }
            }
            set
            {
                base[key] = value;
            }
        }

        public bool CheckSetValue(string key, object value)
        {
            if (this.ContainsKey(key))
            {
                return false;
            }
            this[key] = value;
            return true;
        }

        /// <summary>
        /// 重写只是为了避免警告
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is HashObject))
                return false;

            HashObject that = (HashObject)obj;
            if (this.Count != that.Count)
                return false;
            foreach (string name in this.Keys)
            {
                if (!that.ContainsKey(name))
                    return false;
                object aValue = this[name];
                object bValue = that[name];
                if ((aValue != null) || (bValue != null))
                {
                    if (!object.Equals(this[name], that[name]))
                        return false;
                }
            }

            return true;
        }

        private static void InternalAdd(HashObject obj, string[] keys, object[] values)
        {
            if (keys.Length != values.Length)
                throw new InvalidOperationException("Keys和Values的长度不一致！");
            if (keys.Length == 0)
                throw new InvalidOperationException("添加数据必须有一项！");

            for (int i = 0; i < keys.Length; i++)
            {
                obj.Add(keys[i], values[i]);
            }
        }

        /// <summary>
        /// 拷贝所有项到
        /// </summary>
        /// <param name="dict"></param>
        public void CopyTo(IDictionary<string, object> dict)
        {
            foreach (KeyValuePair<string, object> pair in this)
            {
                dict[pair.Key] = pair.Value;
            }
        }
        /// <summary>
        /// 取指定的值，默认值为null或0或false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            return GetValue<T>(key, default(T));
        }

        /// <summary>
        /// 取指定的值，找不到则返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, T defaultValue)
        {
            object value;
            if (this.TryGetValue(key, out value))
            {
                Type type = typeof(T);
                Type underlyingType = type;
                if (value != null && ReflectionUtils.IsPrimitiveType(type, out underlyingType))
                {
                    return (T)ChangeType(value, underlyingType);
                }
                else if (value != null && underlyingType != type &&
                    underlyingType.IsEnum && !value.GetType().IsEnum) // 整数转成 Nullable<enum>
                {
                    return (T)Enum.ToObject(underlyingType, value);
                }
                else if (value == null && underlyingType.IsSubclassOf(typeof(ValueType)))
                {
                    return defaultValue; // null转成ulong
                }
                else // 只能强转，可能出错
                {
                    return (T)value;
                }
            }
            else
            {
                return defaultValue;
            }
        }

        internal static object ChangeType(object value, Type type)
        {
            if (type == typeof(bool) && value.GetType() == typeof(string))
            {
                return (string)value == "1";
            }
            return Convert.ChangeType(value, type, null);
        }
    }

    /// <summary>
    /// 哈希对象列表
    /// </summary>
    [Serializable]
    public class HashObjectList : List<IHashObject>, IHashObjectList
    {
        private bool hasLongValue;

        /// <summary>
        /// 默认
        /// </summary>
        public HashObjectList()
            : base()
        {
        }

        /// <summary>
        /// 传入出示大小
        /// </summary>
        /// <param name="capacity"></param>
        public HashObjectList(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// 添加许多项
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        public void Add(string[] keys, params object[] values)
        {
            HashObject obj = new HashObject(keys, values);
            Add(obj);
        }

        /// <summary>
        /// 重写只是为了避免警告
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is HashObjectList))
                return false;

            HashObjectList that = (HashObjectList)obj;
            if (this.Count != that.Count)
                return false;

            for (int i = 0; i < this.Count; i++)
            {
                if (!(this[i].Equals(that[i])))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 取一个范围，参数不对会抛出异常
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public new IHashObjectList GetRange(int index, int count)
        {
            List<IHashObject> list = base.GetRange(index, count);
            IHashObjectList answer = new HashObjectList(count);
            foreach (IHashObject item in list)
            {
                answer.Add(item);
            }
            return answer;
        }

        /// <summary>
        /// 按某个字段排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ascending">是否升序</param>
        public void Sort(string key, bool ascending)
        {
            base.Sort(delegate(IHashObject obj1, IHashObject obj2)
            {
                return ascending ?
                    obj1.GetValue<IComparable>(key).CompareTo(obj2.GetValue<IComparable>(key)) :
                    obj2.GetValue<IComparable>(key).CompareTo(obj1.GetValue<IComparable>(key));
            });
        }

        [NonSerialized]
        private IDictionary<string, Hashtable> indexMap;

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item"></param>
        public new void Add(IHashObject item)
        {
            base.Add(item);
            if (indexMap != null)
            {
                AddIndexItem(item);
            }
        }

        /// <summary>
        /// 插入项
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public new void Insert(int index, IHashObject item)
        {
            base.Insert(index, item);
            if (indexMap != null)
            {
                AddIndexItem(item);
            }
        }

        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public new bool Remove(IHashObject item)
        {
            bool answer = base.Remove(item);
            if (indexMap != null)
            {
                RemoveIndexItem(item);
            }
            return answer;
        }

        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="index"></param>
        public new void RemoveAt(int index)
        {
            if (indexMap != null)
            {
                RemoveIndexItem(this[index]);
            }
            base.RemoveAt(index);
        }

        /// <summary>
        /// 移除范围
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            indexMap = null; // 清除索引
        }

        /// <summary>
        /// 移除特定
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public new int RemoveAll(Predicate<IHashObject> match)
        {
            indexMap = null; // 清除索引
            return RemoveAll(match);
        }

        /// <summary>
        /// 移除范围
        /// </summary>
        /// <param name="collection"></param>
        public new void AddRange(IEnumerable<IHashObject> collection)
        {
            base.AddRange(collection);
            indexMap = null; // 清除索引
        }

        /// <summary>
        /// 插入范围
        /// </summary>
        /// <param name="index"></param>
        /// <param name="collection"></param>
        public new void InsertRange(int index, IEnumerable<IHashObject> collection)
        {
            base.InsertRange(index, collection);
            indexMap = null; // 清除索引
        }

        /// <summary>
        /// 清除
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            indexMap = null; // 清除索引
        }


        private void AddIndexItem(IHashObject item)
        {
            if (indexMap == null) return;
            foreach (KeyValuePair<string, Hashtable> pair in indexMap)
            {
                string key = pair.Key;
                Hashtable index = pair.Value;
                object value;
                if (item.TryGetValue(key, out value))
                {
                    index[value] = item;
                }
            }
        }

        private void RemoveIndexItem(IHashObject item)
        {
            if (indexMap == null) return;
            foreach (KeyValuePair<string, Hashtable> pair in indexMap)
            {
                string key = pair.Key;
                Hashtable index = pair.Value;
                object value;
                if (item.TryGetValue(key, out value))
                {
                    index.Remove(value);
                }
            }
        }

        /// <summary>
        /// 建索引快速查找
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IHashObject Find(string key, object value)
        {
            Hashtable index = GetIndex(key);
            IHashObject answer = (IHashObject)index[value];
            if (answer == null && value != null && value.GetType() == typeof(ulong) && hasLongValue) // 对ulong主关键字查找做一次特殊处理，因为从数据库读出来的是long
            {
                long l = Convert.ToInt64(value);
                answer = (IHashObject)index[l];
            }
            return answer;
        }

        private Hashtable GetIndex(string key)
        {
            if (indexMap == null)
            {
                indexMap = new Dictionary<string, Hashtable>();
            }
            Hashtable index;
            if (!indexMap.TryGetValue(key, out index))
            {
                index = new Hashtable();
                indexMap[key] = index;
                BuildIndex(key, index);
            }
            return index;
        }

        private void BuildIndex(string key, Hashtable index)
        {
            hasLongValue = false;
            foreach (IHashObject item in this)
            {
                object value;
                if (item.TryGetValue(key, out value))
                {
                    if (!hasLongValue && value != null && value.GetType() == typeof(long))
                    {
                        hasLongValue = true;
                    }
                    index[value] = item;
                }
            }
        }

    }

}
