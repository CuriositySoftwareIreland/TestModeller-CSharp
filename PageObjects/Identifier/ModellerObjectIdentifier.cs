using CuriositySoftware.PageObjects.Entities;
using CuriositySoftware.PageObjects.Services;
using CuriositySoftware.Utils;

namespace CuriositySoftware.PageObjects.Identifier
{
    public class ModellerObjectIdentifier
    {
        private int pageObjectId { get; set; }

        private PageObjectEntity pageObjectEntity { get; set; }

        public ModellerObjectIdentifier(int pageObjectId)
        {
            this.pageObjectId = pageObjectId;

            this.pageObjectEntity = null;
        }

        public int getPageObjectId()
        {
            return pageObjectId;
        }

        public void setPageObjectId(int pageObjectId)
        {
            this.pageObjectId = pageObjectId;
        }

        public PageObjectEntity getPageObjectEntity(ConnectionProfile conProfile)
        {
            if (pageObjectEntity == null)
            {
                retrieveAndAssignPageObjectEntity(conProfile);
            }

            return pageObjectEntity;
        }

        public void setPageObjectEntity(PageObjectEntity pageObjectEntity)
        {
            this.pageObjectEntity = pageObjectEntity;
        }

        private void retrieveAndAssignPageObjectEntity(ConnectionProfile conProfile)
        {
            PageObjectService poService = new PageObjectService(conProfile);

            this.pageObjectEntity = poService.GetPageObject(this.pageObjectId);
        }
    }
}