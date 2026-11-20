using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Permisstions
{
    public class Permissions
    {
        private Dictionary<int, string> _dic;
        public clsFeatures Features;

        public Permissions(clsFeatures cls)
        {
            _dic = new Dictionary<int, string>();
            Features = cls;

            Type t = typeof(clsFeatures);
            foreach (FieldInfo field in t.GetFields())
            {
                // جلب القيمة الحقيقية المخزنة في الكلاس والاسم مباشرة
                int num = (int)field.GetValue(cls);
                _dic.Add(num, field.Name);
            }
        }

        public void Print()
        {
            Console.WriteLine("-------------------------");
            foreach (var item in _dic.OrderBy(x => x.Key))
            {
                Console.WriteLine($"{item.Key} : {item.Value}");
            }
        }
        // هذه المعادلة تتحقق رياضياً أن الرقم هو قوة للعدد 2 (1, 2, 4, 8, 16, 32, 64, 128, 256, 512...)
        bool IsPowerOfTwo(int x)
        {
            return (x > 0) && ((x & (x - 1)) == 0);
        }
        public void AddNewPermission(string name)
        {
            if (_dic.ContainsValue(name))

            {

                Console.WriteLine("Already added Prmisstion in System");

                return;

            }
            int num = _dic.Last().Key * 2;
            if (!IsPowerOfTwo(num))

            {
                Console.WriteLine("Invalied Number");
                return;

            }

            _dic.Add(num, name);
        }

        public void DropPermission(int num)
        {
            if (!_dic.Remove(num))
            {
                Console.WriteLine("Not found Permission in system.");
            }
        }

        public void Update(int num, string name)
        {
            if (!_dic.ContainsKey(num))
            {
                Console.WriteLine("Not found Permission in system.");
                return;
            }
            if (_dic.ContainsValue(name))
            {
                Console.WriteLine("Permission name already exists.");
                return;
            }
            _dic[num] = name;
        }
        public int GrantPermission(int userPermissions, int permissionToGrant)
        {
            return userPermissions | permissionToGrant;
        }
        public int RevokePermission(int userPermissions, int permissionToRevoke)
        {
            if (userPermissions == Features.All)
            {
                userPermissions = Features.None;
                foreach (var item in _dic.OrderBy(x => x.Key))
                {
                    // فحص هل رقم هذه الصلاحية (item.Key) موجود ضمن صلاحيات المستخدم
                    if ((item.Key & permissionToRevoke) != item.Key && item.Key != Features.All && item.Key != Features.None)
                    {
                        userPermissions = GrantPermission(userPermissions, item.Key);
                    }
                }

            }
            else
            {
                userPermissions = userPermissions & ~permissionToRevoke;
            }
            return userPermissions;


        }
        public bool HasPermission(int userPermissions, int requiredPermission)
        {
            if (userPermissions == -1) return true;
            return (userPermissions & requiredPermission) == requiredPermission;
        }

        public void PrintUserPermissions(int userPermissions)
        {
            if (userPermissions == -1)
            {
                Console.WriteLine("User Permissions: All Permissions (Admin)");
                return;
            }

            if (userPermissions == 0)
            {
                Console.WriteLine("User Permissions: None");
                return;
            }

            List<string> grantedNames = new List<string>();

            foreach (var item in _dic.OrderBy(x => x.Key))
            {
                // فحص هل رقم هذه الصلاحية (item.Key) موجود ضمن صلاحيات المستخدم
                if ((userPermissions & item.Key) == item.Key && item.Key != Features.None)
                {
                    grantedNames.Add(item.Value);
                }
            }

            if (grantedNames.Count > 0)
            {
                Console.WriteLine("User Permissions: " + string.Join(", ", grantedNames));
            }
            else
            {
                Console.WriteLine("User Permissions: Unknown or Empty");
            }
        }
    }

    public class clsFeatures
    {
        public int All = -1;
        public int None = 0;
        public int Customers = 1;
        public int Products = 2;
        public int Insert = 4;
        public int Delete = 8;
        public int Update = 16;
        public int Read = 32;
        public int Write = 64;
        public int DeleteEmp = 128;

    }
}
