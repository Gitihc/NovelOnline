using Repository.Domain;
using Repository.Interface;
using System.Linq;

namespace NovelOnline.App
{
    public class UserNovelManager : BaseApp<UserNovel>
    {
        public UserNovelManager(IUnitWork unitWork, IRepository<UserNovel> repository) : base(unitWork, repository)
        {

        }

        public void AddRelationShip(string userId, string userName, string novelId, string novelName)
        {

            Repository.Add(new UserNovel()
            {
                UserId = userId,
                UserName = userName,
                NovelId = novelId,
                NovelName = novelName
            });
        }

        public void RemoveRelationShip(string userId, string novelId)
        {
            Repository.Delete(x => x.UserId == userId && x.NovelId == novelId);
        }

        /// <summary>
        /// 返回引用Novel数量
        /// </summary>
        /// <param name="noveId"></param>
        /// <returns></returns>
        public long GetCount(string noveId)
        {
            return Repository.Find(x => x.NovelId == noveId).Count();
        }

        public IQueryable<string> GetUserNovelIds(string userId)
        {
            return Repository.Find(x => x.UserId == userId).Select(x => x.NovelId);
        }

        public void RecordLastOpenTime(string userId, string novelId, string chapterId)
        {
            Repository.Update(r => r.UserId == userId && r.NovelId == novelId, r => new UserNovel { LastOpenTime = System.DateTime.Now, LastChapterId = chapterId });
        }
    }
}
