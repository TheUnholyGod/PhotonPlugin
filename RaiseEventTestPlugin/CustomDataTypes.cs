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
        public enum DataTypes
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
            LIST,
            NULL,
        }
        public List<FieldInfo> _fields = new List<FieldInfo>();
        public List<DataTypes> _types = new List<DataTypes>();
        public byte id = 0;
        public string targetReceiverName = "\0";
        public string objectName = "\0";
        public string message = "\0";

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
            Console.Write("\n" + _object._fields[_index].Name);
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
                    _object._fields[_index].SetValue(_object, br.ReadSingle());

                    break;
                case DataTypes.DOUBLE:
                    _object._fields[_index].SetValue(_object, br.ReadDouble());

                    break;
                case DataTypes.STRING:
                    _object._fields[_index].SetValue(_object, br.ReadString());

                    break;
                case DataTypes.LIST:
                    List<int> list = new List<int>();
                    int size = br.ReadInt32();
                    for (int i = 0; i < size; ++i)
                    {
                        list.Add(br.ReadInt32());
                    }

                    _object._fields[_index].SetValue(_object, list);

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
                case DataTypes.LIST:
                    List<int> li = (List<int>)_obj;
                    _bw.Write(li.Count);

                    foreach (int i in li)
                    {
                        _bw.Write(i);
                    }
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
                case "single":
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
                case "list`1":
                    {
                        _t = DataTypes.LIST;
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

                if (i.Name == type.GetField("_fields").Name || i.Name == type.GetField("_types").Name)
                    continue;
                DataTypes _datat = GetVariableType(i.GetValue(this));
                if (_datat != DataTypes.NULL)
                {
                    _types.Add(_datat);
                    _fields.Add(i);
                }
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
        public float posx = 0.0f
            , posy = 0.0f
            , posz = 0.0f,
            hp = 0.0f;
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
        public float damage = 0.0f,
            hp = 0.0f,
            maxhp = 0.0f,
            speed = 0.0f,
            posx = 0.0f,
            posy = 0.0f,
            posz = 0.0f,
            newposx = 0.0f,
            newposz = 0.0f,
            playerposx = 0.0f,
            playerposz = 0.0f,
            playerhp = 0.0f;



        public static object Deserialize(byte[] data)
        {
            MonsterInfo customObject = new MonsterInfo();
            CustomObject.GetFromBinary(data, customObject);
            return customObject;

        }
    }

    class PartitioningInfo : CustomObject
    {
        public int currtile = 0;
        public float playerposx = 0.0f,
        playerposz = 0.0f;
        public string objname = "";
        public static object Deserialize(byte[] data)
        {
            PartitioningInfo customObject = new PartitioningInfo();
            CustomObject.GetFromBinary(data, customObject);
            return customObject;

        }
    }

    class ListPartitioningInfo : PartitioningInfo
    {
        public List<int> _multitile = new List<int>();
        public static object Deserialize(byte[] data)
        {
            ListPartitioningInfo customObject = new ListPartitioningInfo();
            CustomObject.GetFromBinary(data, customObject);
            return customObject;
        }
    }
}
