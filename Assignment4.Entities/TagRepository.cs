using System.Collections.Generic;
using System.Linq;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class TagRepository : ITagRepository
    {
        private readonly KanbanContext _context;

        public TagRepository(KanbanContext context)
        {
            _context = context;
        }

        public (Response Response, int TagId) Create(TagCreateDTO tag)
        {
            var checkTag = _context.Tags.Where(t => t.Name == tag.Name).FirstOrDefault();
            if (checkTag == null)
            {
                return (Response.Conflict, -1);
            }
            var entity = new Tag { Name = tag.Name };

            _context.Tags.Add(entity);
            _context.SaveChanges();

            return (Response.Created, entity.Id);
        }

        public Response Delete(int tagId, bool force = false)
        {
            var isAssigned = _context.Tasks.Any(t => t.Tags.Select(ta => ta.Id).Contains(tagId));

            if (isAssigned == false || force == true)
            {
                var entity = _context.Tags.Find(tagId);

                if (entity == null)
                {
                    return Response.NotFound;
                }
                _context.Tags.Remove(entity);
                _context.SaveChanges();

                return Response.Deleted;
            }
            else
            {
                return Response.Conflict;
            }
        }

        public TagDTO Read(int tagId)
        {
            var tags = from t in _context.Tags
                       where t.Id == tagId
                       select new TagDTO(t.Id, t.Name);

            return tags.FirstOrDefault<TagDTO>();
        }

        public IReadOnlyCollection<TagDTO> ReadAll()
        {
            return _context.Tags.Select(t => new TagDTO(t.Id, t.Name)).ToList().AsReadOnly();
        }

        public Response Update(TagUpdateDTO tag)
        {
            var entity = _context.Tags.Find(tag.Id);

            if (entity == null)
            {
                return Response.NotFound;
            }
            entity.Id = tag.Id;
            entity.Name = tag.Name;

            _context.SaveChanges();

            return Response.Updated;
        }
    }
}