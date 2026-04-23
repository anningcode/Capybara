using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NexusNetNetwork
{
    public class archive_stream
    {
        public archive_stream()
        {
        }
        public archive_stream(List<byte> value)
        {
            data_ = value;
        }
        public enum binary_type_enum
        {
            INT = 1,
            INT64,
            BOOL,
            FLOAT,
            DOUBLE,
            UCHAR,
            STRING,
            DATETIME,
            STRUCT,
            LISTINT,
            LISTUCHAR,
            LISTSTRING,
            LISTSTRUCT
        };
        private static int count_bytes(int num)
        {
            int bytes = 0;
            while (num > 0)
            {
                bytes++;
                num >>= 8;
            }
            return bytes;
        }
        private static int count_param(List<byte> value)
        {
            int result = 0;
            while (value.Count != 0)
            {
                result++;
                int count = value[0] & 7;
                long size = 0;
                for (int i = 0; i < count; ++i)
                {
                    size += (long)value[i + 1] << 8 * i;
                }
                value.RemoveRange(0, (int)(count + size + 1));
            }
            return result;
        }
        private static void to_binary_t(binary_type_enum type, long value, ref List<byte> result)
        {
            byte tag = (byte)((int)type << 4);
            List<byte> tmp = new List<byte>();
            if (value < 0)
            {
                tag += 1 << 3;
                value = -1 * value;
            }

            do
            {
                tmp.Add((byte)value);
                value >>= 8;

            } while (value > 0);
            int size = tmp.Count;
            int count = count_bytes(size);
            tag += (byte)count;
            result.Add(tag);
            for (int i = 0; i < count; ++i)
            {
                result.Add((byte)(size >> 8 * i));
            }
            result.AddRange(tmp);
        }
        private static void to_binary_t(binary_type_enum type, List<byte> value, ref List<byte> result)
        {
            byte tag = (byte)((int)type << 4);
            int size = value.Count;
            int count = count_bytes(size);
            tag += (byte)count;
            result.Add(tag);
            for (int i = 0; i < count; ++i)
            {
                result.Add((byte)(size >> 8 * i));
            }
            result.AddRange(value);
        }
        private static void to_binary(int value, ref List<byte> result)
        {
            to_binary_t(binary_type_enum.INT, value, ref result);
        }
        private static void to_binary(long value, ref List<byte> result)
        {
            to_binary_t(binary_type_enum.INT64, value, ref result);
        }
        private static void to_binary(bool value, ref List<byte> result)
        {
            to_binary_t(binary_type_enum.BOOL, value ? 1 : 0, ref result);
        }
        private static void to_binary(float value, ref List<byte> result)
        {
            long tmp = (long)(value * 1000000.0);
            to_binary_t(binary_type_enum.FLOAT, tmp, ref result);
        }
        private static void to_binary(double value, ref List<byte> result)
        {
            long tmp = (long)(value * 10000000000000000.0);
            to_binary_t(binary_type_enum.DOUBLE, tmp, ref result);
        }
        private static void to_binary(byte value, ref List<byte> result)
        {
            to_binary_t(binary_type_enum.UCHAR, value, ref result);
        }
        private static void to_binary(DateTime value, ref List<byte> result)
        {
            DateTime begin = new(1970, 1, 1, 8, 0, 0);
            long timestampMillis = Convert.ToInt64(value.Subtract(begin).TotalMilliseconds);
            to_binary_t(binary_type_enum.DATETIME, timestampMillis, ref result);
        }
        private static void to_binary(string value, ref List<byte> result)
        {
            List<byte> tmp = Encoding.UTF8.GetBytes(value).ToList();
            to_binary_t(binary_type_enum.STRING, tmp, ref result);
        }
        private static void to_binary(List<byte> value, ref List<byte> result)
        {
            to_binary_t(binary_type_enum.LISTUCHAR, value, ref result);
        }
        public static archive_stream operator <<(archive_stream ths, int value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, long value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, bool value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, float value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, double value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, byte value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, DateTime value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, string value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, List<byte> value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, object value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, List<int> value)
        {
            ths.serialize(value);
            return ths;
        }
        public static archive_stream operator <<(archive_stream ths, List<string> value)
        {
            ths.serialize(value);
            return ths;
        }
        public archive_stream serialize(int value)
        {
            to_binary(value, ref data_);
            return this;
        }
        public archive_stream serialize(long value)
        {
            to_binary(value, ref data_);
            return this;
        }
        public archive_stream serialize(bool value)
        {
            to_binary(value, ref data_);
            return this;
        }
        public archive_stream serialize(float value)
        {
            to_binary(value, ref data_);
            return this;
        }
        public archive_stream serialize(double value)
        {
            to_binary(value, ref data_);
            return this;
        }
        public archive_stream serialize(byte value)
        {
            to_binary(value, ref data_);
            return this;
        }
        public archive_stream serialize(DateTime value)
        {
            to_binary(value, ref data_);
            return this;
        }
        public archive_stream serialize(string value)
        {
            to_binary(value, ref data_);
            return this;
        }
        public archive_stream serialize(List<byte> value)
        {
            to_binary(value, ref data_);
            return this;
        }
        public archive_stream serialize(List<object> value)
        {
            archive_stream stream = new archive_stream();
            foreach (object it in value)
            {
                stream.serialize(it);
            }
            List<byte> values = stream.data();
            to_binary_t(binary_type_enum.LISTSTRUCT, values, ref data_);
            return this;
        }
        public archive_stream serialize(object value)
        {
            archive_stream stream = new archive_stream();
            Type type = value.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(int))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = 0;
                    }
                    stream.serialize((int)tmp);
                }
                else if (property.PropertyType == typeof(long))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = 0;
                    }
                    stream.serialize((long)tmp);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = false;
                    }
                    stream.serialize((bool)tmp);
                }
                else if (property.PropertyType == typeof(float))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = 0.0;
                    }
                    stream.serialize((float)tmp);
                }
                else if (property.PropertyType == typeof(double))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = 0.00;
                    }
                    stream.serialize((double)tmp);
                }
                else if (property.PropertyType == typeof(byte))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = 0;
                    }
                    stream.serialize((byte)tmp);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = new DateTime(1970, 1, 1, 8, 0, 0);
                    }
                    stream.serialize((DateTime)tmp);
                }
                else if (property.PropertyType == typeof(string))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = "";
                    }
                    stream.serialize((string)tmp);
                }
                else if (property.PropertyType == typeof(List<byte>))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = new List<byte>();
                    }
                    stream.serialize((List<byte>)tmp);
                }
                else if (property.PropertyType == typeof(List<int>))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = new List<int>();
                    }
                    stream.serialize((List<int>)tmp);
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = new List<string>();
                    }
                    stream.serialize((List<string>)tmp);
                }
                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = Activator.CreateInstance(property.PropertyType);
                    }
                    PropertyInfo count_property = property.PropertyType.GetProperty("Count");
                    int count = (int)count_property.GetValue(tmp);
                    PropertyInfo item_property = property.PropertyType.GetProperty("Item");
                    List<object> list_data = new List<object>();
                    for (int i = 0; i < count; i++)
                    {
                        object item = item_property.GetValue(tmp, new object[] { i });
                        list_data.Add(item);
                    }
                    stream.serialize(list_data);
                }
                else if (property.PropertyType.IsClass)
                {
                    object tmp = property.GetValue(value);
                    if (tmp == null)
                    {
                        tmp = Activator.CreateInstance(property.PropertyType);
                    }
                    stream.serialize(tmp);
                }
                else
                {
                    throw new Exception("data type error.");
                }
            }
            List<byte> values = stream.data();
            to_binary_t(binary_type_enum.STRUCT, values, ref data_);
            return this;
        }
        public archive_stream serialize(List<int> value)
        {
            archive_stream stream = new archive_stream();
            foreach (int it in value)
            {
                stream.serialize(it);
            }
            List<byte> values = stream.data();
            to_binary_t(binary_type_enum.LISTINT, values, ref data_);
            return this;
        }
        public archive_stream serialize(List<string> value)
        {
            archive_stream stream = new archive_stream();
            foreach (string it in value)
            {
                stream.serialize(it);
            }
            List<byte> values = stream.data();
            to_binary_t(binary_type_enum.LISTSTRING, values, ref data_);
            return this;
        }
        private static void binary_to_t(ref List<byte> value, binary_type_enum type, ref long result)
        {
            if (!value.Any() || value[0] >> 4 != (byte)type)
            {
                if ((byte)type == 1 && value[0] >> 4 != 2)
                {
                    throw new Exception("data type error");
                }
            }
            result = 0;
            bool symbol = 0 != (value[0] & 8);
            int count = value[0] & 7;
            long size = 0;
            for (int i = 0; i < count; ++i)
            {
                size += (long)value[i + 1] << 8 * i;
            }
            List<byte> tmp = value.Skip(1 + count).Take((int)size).ToList();
            for (int i = 0; i < tmp.Count; ++i)
            {
                result += (long)tmp[i] << 8 * i;
            }
            if (symbol)
            {
                result *= -1;
            }
            value.RemoveRange(0, (int)(count + size + 1));
        }
        private static void binary_to_t(ref List<byte> value, binary_type_enum type, ref List<byte> result)
        {
            if (!value.Any() || value[0] >> 4 != (byte)type)
            {
                throw new Exception("data type error");
            }
            int count = value[0] & 7;
            long size = 0;
            for (int i = 0; i < count; ++i)
            {
                size += (long)value[i + 1] << 8 * i;
            }
            result = value.Skip(1 + count).Take((int)size).ToList();
            value.RemoveRange(0, (int)(count + size + 1));
        }
        private static void binary_to(ref List<byte> value, ref int result)
        {
            long tmp = 0;
            binary_to_t(ref value, binary_type_enum.INT, ref tmp);
            result = (int)tmp;
        }
        private static void binary_to(ref List<byte> value, ref long result)
        {
            binary_to_t(ref value, binary_type_enum.INT64, ref result);
        }
        private static void binary_to(ref List<byte> value, ref bool result)
        {
            long tmp = 0;
            binary_to_t(ref value, binary_type_enum.BOOL, ref tmp);
            result = tmp == 1;
        }
        private static void binary_to(ref List<byte> value, ref float result)
        {
            long tmp = 0;
            binary_to_t(ref value, binary_type_enum.FLOAT, ref tmp);
            result = (float)tmp / 1000000;
        }
        private static void binary_to(ref List<byte> value, ref double result)
        {
            long tmp = 0;
            binary_to_t(ref value, binary_type_enum.DOUBLE, ref tmp);
            result = (double)tmp / 10000000000000000;
        }
        private static void binary_to(ref List<byte> value, ref byte result)
        {
            long tmp = 0;
            binary_to_t(ref value, binary_type_enum.UCHAR, ref tmp);
            result = (byte)tmp;
        }
        private static void binary_to(ref List<byte> value, ref DateTime result)
        {
            long tmp = 0;
            binary_to_t(ref value, binary_type_enum.DATETIME, ref tmp);
            DateTime dateTime = new DateTime(1970, 1, 1, 8, 0, 0, 0);
            result = dateTime.AddMilliseconds(tmp);
        }
        private static void binary_to(ref List<byte> value, ref string result)
        {
            List<byte> tmp = new List<byte>();
            binary_to_t(ref value, binary_type_enum.STRING, ref tmp);
            result = Encoding.UTF8.GetString(tmp.ToArray());
        }
        private static void binary_to(ref List<byte> value, ref List<byte> result)
        {
            binary_to_t(ref value, binary_type_enum.STRING, ref result);
        }
        private static void binary_to(ref List<byte> value, ref List<int> result)
        {
            if (!value.Any() || value[0] >> 4 != (byte)binary_type_enum.LISTINT)
            {
                throw new Exception("data type error");
            }
            result.Clear();
            int count = value[0] & 7;
            long size = 0;
            for (int i = 0; i < count; ++i)
            {
                size += (long)value[i + 1] << 8 * i;
            }
            List<byte> data = value.Skip(count + 1).Take((int)size).ToList();
            while (data.Any())
            {
                int tmp = 0;
                binary_to(ref data, ref tmp);
                result.Add(tmp);
            }
            value.RemoveRange(0, (int)(count + size + 1));
        }
        private static void binary_to(ref List<byte> value, ref List<string> result)
        {
            if (!value.Any() || value[0] >> 4 != (byte)binary_type_enum.LISTSTRING)
            {
                throw new Exception("data type error");
            }
            result.Clear();
            int count = value[0] & 7;
            long size = 0;
            for (int i = 0; i < count; ++i)
            {
                size += (long)value[i + 1] << 8 * i;
            }
            List<byte> data = value.Skip(count + 1).Take((int)size).ToList();
            while (data.Any())
            {
                string tmp = "";
                binary_to(ref data, ref tmp);
                result.Add(tmp);
            }
            value.RemoveRange(0, (int)(count + size + 1));
        }
        private static void binary_to(ref List<byte> value, ref IList result)
        {
            if (!value.Any() || value[0] >> 4 != (byte)binary_type_enum.LISTSTRUCT)
            {
                throw new Exception("data type error");
            }
            int count = value[0] & 7;
            long size = 0;
            for (int i = 0; i < count; ++i)
            {
                size += (long)value[i + 1] << 8 * i;
            }
            List<byte> data = value.Skip(count + 1).Take((int)size).ToList();
            Type type = result.GetType().GetGenericArguments()[0];
            while (data.Any())
            {
                object item = Activator.CreateInstance(type);
                binary_to(ref data, ref item);
                result.Add(item);
            }
        }
        private static void binary_to(ref List<byte> value, ref object result)
        {
            if (!value.Any() || value[0] >> 4 != (byte)binary_type_enum.STRUCT)
            {
                throw new Exception("data type error");
            }
            int count = value[0] & 7;
            long size = 0;
            for (int i = 0; i < count; ++i)
            {
                size += (long)value[i + 1] << 8 * i;
            }
            List<byte> data = value.Skip(count + 1).Take((int)size).ToList();
            Type type = result.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(int))
                {
                    int tmp = 0;
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType == typeof(long))
                {
                    long tmp = 0;
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    bool tmp = false;
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType == typeof(float))
                {
                    float tmp = 0;
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType == typeof(double))
                {
                    double tmp = 0;
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType == typeof(byte))
                {
                    byte tmp = 0;
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    DateTime tmp = new DateTime(1970, 1, 1, 8, 0, 0);
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType == typeof(string))
                {
                    string tmp = "";
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType == typeof(List<byte>))
                {
                    List<byte> tmp = new List<byte>();
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType == typeof(List<int>))
                {
                    List<int> tmp = new List<int>();
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    List<string> tmp = new List<string>();
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var tmp = Activator.CreateInstance(property.PropertyType);
                    Type ilist_type = typeof(List<>);
                    Type constructed_list_type = ilist_type.MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                    IList tmplist = (IList)Activator.CreateInstance(constructed_list_type);
                    binary_to(ref data, ref tmplist);
                    Type list_type = tmp.GetType();
                    MethodInfo add_method = list_type.GetMethod("Add");
                    foreach (var item in tmplist)
                    {
                        add_method.Invoke(tmp, new object[] { item });
                    }
                    property.SetValue(result, tmp, null);
                }
                else if (property.PropertyType.IsClass)
                {
                    var tmp = Activator.CreateInstance(property.PropertyType);
                    binary_to(ref data, ref tmp);
                    property.SetValue(result, tmp, null);
                }
                else
                {
                    throw new Exception("data type error.");
                }
            }
            value.RemoveRange(0, (int)(count + size + 1));
        }
        public archive_stream deserialize(ref int value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref long value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref bool value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref float value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref double value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref byte value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref DateTime value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref string value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref List<byte> value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref object value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref List<int> value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public archive_stream deserialize(ref List<string> value)
        {
            binary_to(ref data_, ref value);
            return this;
        }
        public List<byte> data()
        {
            return data_;
        }
        public bool empty()
        {
            return data_.Count == 0;
        }
        public void clear()
        {
            data_.Clear();
        }
        public void add(archive_stream value)
        {
            data_.AddRange(value.data_);
        }
        public void add(List<byte> data)
        {
            data_.AddRange(data);
        }
        private List<byte> data_ = new List<byte>();
        public class function_manage
        {
            public void bind(string name, MethodInfo method, object obj)
            {
                var add = new List<Tuple<MethodInfo, object>>();
                add.Add(new Tuple<MethodInfo, object>(method, obj));
                func_list_[name] = add;
            }
            public archive_stream invoke(string name, archive_stream values)
            {
                archive_stream result = new archive_stream();
                if (func_list_.ContainsKey(name))
                {
                    if (func_list_[name].Count > 0)
                    {
                        ParameterInfo[] paramlist = func_list_[name][0].Item1.GetParameters();
                        List<object> param_list = new List<object>();
                        foreach (ParameterInfo param in paramlist)
                        {
                            if (param.ParameterType == typeof(int))
                            {
                                int tmp = 0;
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType == typeof(long))
                            {
                                long tmp = 0;
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType == typeof(bool))
                            {
                                bool tmp = false;
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType == typeof(float))
                            {
                                float tmp = 0;
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType == typeof(double))
                            {
                                double tmp = 0;
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType == typeof(byte))
                            {
                                byte tmp = 0;
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType == typeof(DateTime))
                            {
                                DateTime tmp = new DateTime();
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType == typeof(string))
                            {
                                string tmp = "";
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType == typeof(List<byte>))
                            {
                                List<byte> tmp = new List<byte>();
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType == typeof(List<int>))
                            {
                                List<int> tmp = new List<int>();
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType == typeof(List<string>))
                            {
                                List<string> tmp = new List<string>();
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else if (param.ParameterType.IsClass)
                            {
                                object tmp = Activator.CreateInstance(param.ParameterType);
                                values.deserialize(ref tmp);
                                param_list.Insert(0, tmp);
                            }
                            else
                            {
                                throw new Exception("data type error.");
                            }
                        }
                        object ret = func_list_[name][0].Item1.Invoke(func_list_[name][0].Item2, param_list.ToArray());
                        if (ret == null)
                        {

                        }
                        else if (ret.GetType() == typeof(int))
                        {
                            result.serialize((int)ret);
                        }
                        else if (ret.GetType() == typeof(long))
                        {
                            result.serialize((long)ret);
                        }
                        else if (ret.GetType() == typeof(bool))
                        {
                            result.serialize((bool)ret);
                        }
                        else if (ret.GetType() == typeof(float))
                        {
                            result.serialize((float)ret);
                        }
                        else if (ret.GetType() == typeof(double))
                        {
                            result.serialize((double)ret);
                        }
                        else if (ret.GetType() == typeof(byte))
                        {
                            result.serialize((byte)ret);
                        }
                        else if (ret.GetType() == typeof(DateTime))
                        {
                            result.serialize((DateTime)ret);
                        }
                        else if (ret.GetType() == typeof(string))
                        {
                            result.serialize((string)ret);
                        }
                        else if (ret.GetType() == typeof(List<byte>))
                        {
                            result.serialize((List<byte>)ret);
                        }
                        else if (ret.GetType() == typeof(List<int>))
                        {
                            result.serialize((List<int>)ret);
                        }
                        else if (ret.GetType() == typeof(List<string>))
                        {
                            result.serialize((List<string>)ret);
                        }
                        else if (ret.GetType() == typeof(void))
                        {

                        }
                        else
                        {
                            result.serialize(ret);
                        }
                    }
                }
                else
                {
                    throw new Exception("method type error.");
                }
                return result;
            }
            private Dictionary<string, List<Tuple<MethodInfo, object>>> func_list_ = new Dictionary<string, List<Tuple<MethodInfo, object>>>();
        }
    }
}
