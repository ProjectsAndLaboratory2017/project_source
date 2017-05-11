using System.ServiceModel;

namespace WebServiceWCF
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        Person GetData(string id);
    }
}