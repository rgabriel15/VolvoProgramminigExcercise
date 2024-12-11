using Web.UI.Services.Base.Service.Models;

namespace Web.UI.Services.Base.Service.Interfaces;
internal interface IBaseService<T> where T : BaseModel
{
    Task<T> AddAsync(T model);
    Task<ulong> DeleteAsync(ulong id);
    Task<BaseListModel<T>> ListAsync(
        uint pageNumber = BaseListModel<BaseModel>.DefaultPageNumber
        , ushort pageSize = BaseListModel<BaseModel>.DefaultPageSize);
    Task<T> UpdateAsync(T model);
}
