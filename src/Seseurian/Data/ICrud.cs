using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seseurian.Data
{
    public interface ICrud<T> where T:class
    {
        bool InsertData(T data);
        bool UpdateData(T data);
        List<T> GetAllData();

        List<T> FindByKeyword(string Keyword);
        T GetDataById(string Id);
        bool DeleteData(T item);
      
    }
}
