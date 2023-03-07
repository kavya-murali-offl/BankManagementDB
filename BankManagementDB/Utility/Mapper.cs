using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Utility
{
    public class Mapper
    { 
       public static TEntity Map<TBusiness, TEntity>(TBusiness businessObject) where TBusiness : new() where TEntity : new()  
        {
            TEntity entityObject = new TEntity();

            PropertyInfo[] businessProperties = typeof(TBusiness).GetProperties();
            PropertyInfo[] entityProperties = typeof(TEntity).GetProperties();

            foreach (PropertyInfo businessProperty in businessProperties)
            {
                foreach (PropertyInfo entityProperty in entityProperties)
                {
                    if (entityProperty.Name == businessProperty.Name)
                    {
                        entityProperty.SetValue(entityObject, businessProperty.GetValue(businessObject));
                        break;
                    }
                }
            }

            return entityObject;
        }
    }
}
