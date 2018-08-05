using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace TestPlugin
{
    public class CustomObject
    {
        protected enum DataTypes
        {
            BYTE,
            BOOL,
            SHORT,
            INT,
            LONG,
            FLOAT,
            DOUBLE,
            STRING,
            OBJECT_ARRAY,
            BYTE_ARRAY,
            ARRAY,
            HASHTABLE,
            DICTIONARY_OO,
            DICTIONARY_OV,
            DICTIONARY_KO,
            DICTIONARY_KV,
            NULL,
        }
        List<DataTypes> _types = new List<DataTypes>();
        List<FieldInfo> _fields = new List<FieldInfo>();
        public byte id { get; set; } = 0;
        public string targetReceiverName = "";
        public string objectName = "";
        public string message = "";

        public static object Deserialize(byte[] data)
        {
            CustomObject customObject = new CustomObject();
            
            GetFromBinary(data, customObject);
            return customObject;

        }

        // For a SerializeMethod, we need a byte-array as result.
        public static byte[] Serialize(object customType)
        {
            var c = (CustomObject)customType;

            byte[] returnObject;
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < c._fields.Count; ++i)
                    {
                        WriteToBinary(c._fields[i].GetValue(c), c._types[i], bw);
                    }
                    returnObject = ms.ToArray();
                }
            }

            return returnObject;
        }

        protected static void GetFromBinary(byte[] _bytes, CustomObject _object)
        {
            _object.Init();

            using (var s = new MemoryStream(_bytes))
            {
                using (var br = new BinaryReader(s))
                {
                    for (int i = 0; i < _object._fields.Count; ++i)
                    {
                        GetFromBinary(i, _object, br);
                    }
                }
            }
        }

        protected static void GetFromBinary(int _index, CustomObject _object, BinaryReader br)
        {
            switch (_object._types[_index])
            {
                case DataTypes.BYTE:
                    _object._fields[_index].SetValue(_object, br.ReadByte());
                    break;
                case DataTypes.BOOL:
                    _object._fields[_index].SetValue(_object, br.ReadBoolean());
                    break;
                case DataTypes.SHORT:
                    _object._fields[_index].SetValue(_object, br.ReadInt16());

                    break;
                case DataTypes.INT:
                    _object._fields[_index].SetValue(_object, br.ReadInt32());

                    break;
                case DataTypes.LONG:
                    _object._fields[_index].SetValue(_object, br.ReadInt64());

                    break;
                case DataTypes.FLOAT:
                    _object._fields[_index].SetValue(_object, (float)br.ReadDouble());

                    break;
                case DataTypes.DOUBLE:
                    _object._fields[_index].SetValue(_object, br.ReadDouble());

                    break;
                case DataTypes.STRING:
                    _object._fields[_index].SetValue(_object, br.ReadString());

                    break;
            }
        }

        protected static void WriteToBinary<T>(T _val, DataTypes _type, BinaryWriter _bw)
        {
            object _obj = _val;
            switch (_type)
            {
                case DataTypes.BYTE:
                    _bw.Write((byte)_obj);
                    break;
                case DataTypes.BOOL:
                    _bw.Write((bool)_obj);
                    break;
                case DataTypes.SHORT:
                    _bw.Write((short)_obj);
                    break;
                case DataTypes.INT:
                    _bw.Write((int)_obj);
                    break;
                case DataTypes.LONG:
                    _bw.Write((long)_obj);
                    break;
                case DataTypes.FLOAT:
                    _bw.Write((float)_obj);
                    break;
                case DataTypes.DOUBLE:
                    _bw.Write((double)_obj);
                    break;
                case DataTypes.STRING:
                    _bw.Write((string)_obj);
                    break;
            }
        }

        public void Init()
        {
            this.GetVariableType();
        }

        protected DataTypes GetVariableType<T>(T _var)
        {
            DataTypes _t = DataTypes.NULL;

            System.Type type = _var.GetType();


            switch (type.Name.ToLower())
            {
                case "byte":
                    {
                        _t = DataTypes.BYTE;
                        break;
                    }
                case "boolean":
                    {
                        _t = DataTypes.BOOL;
                        break;
                    }
                case "short":
                    {
                        _t = DataTypes.SHORT;
                        break;
                    }
                case "int32":
                    {
                        _t = DataTypes.INT;
                        break;
                    }
                case "long":
                    {
                        _t = DataTypes.LONG;
                        break;
                    }
                case "float":
                    {
                        _t = DataTypes.FLOAT;
                        break;
                    }
                case "double":
                    {
                        _t = DataTypes.DOUBLE;
                        break;
                    }
                case "string":
                    {
                        _t = DataTypes.STRING;
                        break;
                    }
            }
            return _t;
        }

        public void GetVariableType()
        {
            System.Type type = this.GetType();
            foreach (FieldInfo i in type.GetFields(BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance))
            {
                DataTypes _datat = GetVariableType(i.GetValue(this));
                if (_datat != DataTypes.NULL)
                {
                    _types.Add(_datat);
                    _fields.Add(i);
                }
                Console.Write("{0} - type {1}\n", i.Name,
                           i.GetValue(this).GetType().Name);
            }

            foreach (DataTypes i in _types)
            {
                Console.Write(i.ToString() + "\n");
            }
        }
    }


    class AccountdDetails : CustomObject
    {
        public string userid = "";
        public string pw = "";

        public static object Deserialize(byte[] data)
        {
            AccountdDetails customObject = new AccountdDetails();
            CustomObject.GetFromBinary(data, customObject);
            return customObject;

        }
    }

    class PlayerPos : CustomObject
    {
        public string userid = "";
        public int posx = 0
            , posy = 0
            , posz = 0;
        public string guild = "";
        public static object Deserialize(byte[] data)
        {
            PlayerPos customObject = new PlayerPos();
            CustomObject.GetFromBinary(data, customObject);
            return customObject;

        }
    }

    class MonsterInfo : CustomObject
    {
        public string name = "";
        public int damage = 0,
            hp = 0,
            maxhp = 0,
            speed = 0, 
            posx = 0,
            posy = 0,
            posz = 0;

        public static object Deserialize(byte[] data)
        {
            MonsterInfo customObject = new MonsterInfo();
            CustomObject.GetFromBinary(data, customObject);
            return customObject;

        }
    }
}
