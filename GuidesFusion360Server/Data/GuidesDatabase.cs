using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Data
{
    public static class GuidesDatabase
    {
        public static async Task<List<Guide>> SelectGuides(string hidden)
        {
            var sql = $"SELECT G.id, G.name, G.description FROM Guides AS G WHERE G.hidden='{hidden}'";
            var query = await Database.SelectAllData(sql);

            var guides = new List<Guide>();
            foreach (var guide in query)
            {
                guides.Add(new Guide()
                {
                    Id = Int32.Parse(guide[0]),
                    Name = guide[1],
                    Description = guide[2]
                });
            }

            return guides;
        }

        public static async Task<string> SelectGuideAccess(int id)
        {
            var sql = $"SELECT G.hidden FROM Guides AS G WHERE G.id={id}";
            var query = await Database.SelectRowData(sql);

            return query[0];
        }

        public static async Task<List<PartGuide>> SelectPartGuides(int guideId)
        {
            var sql =
                "SELECT PG.id, PG.name, PG.content, PG.sortKey FROM PartGuides as PG " +
                $"WHERE PG.guideId = {guideId} ORDER BY PG.sortKey ASC";
            var query = await Database.SelectAllData(sql);
            
            var guides = new List<PartGuide>();
            foreach (var guide in query)
            {
                guides.Add(new PartGuide()
                {
                    Id = Int32.Parse(guide[0]),
                    Name = guide[1],
                    Content = guide[2],
                    SortKey = Int32.Parse(guide[3])
                });
            }

            return guides;
        }

        public static async Task<PartGuide> SelectPartGuide(int id)
        {
            var sql =
                "SELECT PG.id, PG.name, PG.content, PG.sortKey, PG.guideId " +
                $"FROM PartGuides as PG WHERE PG.id = {id}";
            var query = await Database.SelectRowData(sql);

            return new PartGuide()
            {
                Id = Int32.Parse(query[0]),
                Name = query[1],
                Content = query[2],
                SortKey = Int32.Parse(query[3]),
                GuideId = Int32.Parse(query[4])
            };
        }

        public static async Task<int> InsertGuide(string name, string description, int ownerId)
        {
            var sql =
                "INSERT INTO Guides (name, description, ownerId, hidden) " +
                $"VALUES ('{name}', '{description}', '{ownerId}', 'true')";
            var lastId = await Database.ChangeData(sql);

            return lastId;
        }

        public static async Task<int> InsertPartGuide(int guideId, string name, string content, int sortKey)
        {
            var sql =
                "INSERT INTO PartGuides (guideId, name, content, sortKey) " +
                $"VALUES ('{guideId}', '{name}', '{content}', '{sortKey}')";
            var lastId = await Database.ChangeData(sql);

            return lastId;
        }

        public static async Task<int> UpdateGuideHidden(int id, string hidden)
        {
            var sql = $"UPDATE GUIDES SET hidden='{hidden}' WHERE id={id}";
            var status = await Database.ChangeData(sql);

            return status;
        }

        public static async Task<int> UpdatePartGuide(int id, string name, string content)
        {
            var sql = $"UPDATE PartGuides SET name='{name}', content='{content}' WHERE id={id}";
            var status = await Database.ChangeData(sql);

            return status;
        }

        public static async Task<int> UpdatePartGuideSortKey(int id, int sortKey)
        {
            var sql = $"UPDATE PartGuides SET sortKey='{sortKey}' WHERE id={id}";
            var status = await Database.ChangeData(sql);

            return status;
        }
    }
}
