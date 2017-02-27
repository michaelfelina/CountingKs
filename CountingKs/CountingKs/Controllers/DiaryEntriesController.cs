using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;

namespace CountingKs.Controllers
{
    public class DiaryEntriesController : BaseApiController
    {
        private ICountingKsIdentityService _identityService;

        public DiaryEntriesController(ICountingKsRepository repo, ICountingKsIdentityService identityService)
            : base(repo)
        {
            _identityService = identityService;
        }

        public IEnumerable<DiaryEntryModel> Get(DateTime diaryId)
        {
            string username = _identityService.CurrentUser;
            var result = repo.GetDiaryEntries(username, diaryId.Date)
                .ToList()
                .Select(d => modelFactory.Create(d));

            return result;
        }

        public HttpResponseMessage Get(DateTime diaryId, int id)
        {
            string username = _identityService.CurrentUser;
            var result = repo.GetDiaryEntry(username, diaryId, id);
            if (result == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, modelFactory.Create(result));
            ;
        }

        public HttpResponseMessage Post(DateTime diaryId, [FromBody] DiaryEntryModel model)
        {
            try
            {
                var entity = modelFactory.Parse(model);
                if (entity == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read diary entry");
                var diary = repo.GetDiary(_identityService.CurrentUser, diaryId);
                if (diary == null) Request.CreateResponse(HttpStatusCode.NotFound);
                if (diary.Entries.Any(e => e.Measure.Id == entity.Measure.Id))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Dupicate not allowed");
                diary.Entries.Add(entity);
                if (repo.SaveAll())
                    return Request.CreateResponse(HttpStatusCode.Created, modelFactory.Create(entity));
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Save Failed");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Delete(DateTime diaryId, int id)
        {
            try
            {
                if (repo.GetDiaryEntries(_identityService.CurrentUser, diaryId).Any(e => e.Id == id) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    if (repo.DeleteDiaryEntry(id) && repo.SaveAll())
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Delete Failed");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut]
        [HttpPatch]
        public HttpResponseMessage Patch(DateTime diaryId, int id, [FromBody] DiaryEntryModel model)
        {
            try
            {
                var entity = repo.GetDiaryEntry(_identityService.CurrentUser, diaryId, id);
                if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                var parsedValue = modelFactory.Parse(model);
                if (parsedValue == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
                if (Math.Abs(entity.Quantity - parsedValue.Quantity) > .01)
                {
                    entity.Quantity = parsedValue.Quantity;
                    if (repo.SaveAll())
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
