using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _01electronics_erp;

namespace _01electronics_crm
{
    /*class RFQ
    {
        const int RFQ_ID_IDENTIFIER_TOKEN = 3;
        const int RFQ_ID_SERIAL_TOKEN = 4;
        const int RFQ_ID_EMPLOYEE_INITIALS_TOKEN = 9;
        const int RFQ_ID_DATE_TOKEN = 8;
        const int RFQ_ID_REVISION_OFFSET_TOKEN = 3;
        const int RFQ_ID_REVISION_SERIAL_TOKEN = 2;

        const int BASE_RFQ_ID_DASH_SEPARATORS = 3;
        const int REVISED_RFQ_ID_DASH_SEPARATORS = BASE_RFQ_ID_DASH_SEPARATORS + 1;

        const int BASE_RFQ_ID_LENGTH =	RFQ_ID_IDENTIFIER_TOKEN + RFQ_ID_SERIAL_TOKEN + RFQ_ID_EMPLOYEE_INITIALS_TOKEN + RFQ_ID_DATE_TOKEN + BASE_RFQ_ID_DASH_SEPARATORS;
        const int REVISED_RFQ_ID_LENGTH = BASE_RFQ_ID_LENGTH + RFQ_ID_REVISION_OFFSET_TOKEN + RFQ_ID_REVISION_SERIAL_TOKEN + 1;

        const int RFQ_ID_SERIAL_START_INDEX	= RFQ_ID_IDENTIFIER_TOKEN + 1;
        const int RFQ_ID_EMPLOYEE_INITIALS_START_INDEX = RFQ_ID_SERIAL_START_INDEX + RFQ_ID_SERIAL_TOKEN + 1;
        const int RFQ_ID_DATE_START_INDEX = RFQ_ID_EMPLOYEE_INITIALS_START_INDEX + RFQ_ID_EMPLOYEE_INITIALS_TOKEN + 1;
        const int RFQ_ID_REVISION_SERIAL_START_INDEX = RFQ_ID_DATE_START_INDEX + RFQ_ID_DATE_TOKEN + RFQ_ID_REVISION_OFFSET_TOKEN + 1;

        String RFQIdString = "RFQ-0001-XXXX.XXXX-DDMMYYYY";
        String revisedRFQIdString = "RFQ-0001-XXXX.XXXX-DDMMYYYY-REV01";

        String RFQIdCString;

        
        
        Contact contact = new Contact();
        Employee assignedEngineer = new Employee();
        //Employee* salesPerson;

        ulong rfqSerial;
        ulong rfqVersion;

        ulong rfqStatusId;

        public List<COMPANY_WORK_MACROS.RFQ_PRODUCT_STRUCT> RFQProductsList = new List<COMPANY_WORK_MACROS.RFQ_PRODUCT_STRUCT>();
        //bool RFQProductsValid[MAX_RFQ_PRODUCTS];

        ulong contractTypeId;

        ulong rfqFailureReasonId;

        ulong noOfSavedRFQProducts;
        
        String contractType;

        DateTime rfqDeadlineDate;
        DateTime rfqIssueDate;
        DateTime rfqRejectionDate;

        String rfqNotes;

        String rfqStatus;
        String rfqFailureReason;
        
        public RFQClass()
        {

        }

        void ResetRFQSerial()
        {
            rfqSerial = 0;
        }
        void ResetRFQVersion()
        {
            rfqVersion = 1;
        }

        void ResetRFQProduct1Type()
        {
            RFQProductsList[0].productType.typeId = 0;
            RFQProductsList[0].productType.typeName = "Other";

            SetNoOfSavedRFQProducts();
        }
        void ResetRFQProduct2Type()
        {
            RFQProductsList[1].productType.typeId = 0;
            RFQProductsList[1].productType.typeName = "Other";

            SetNoOfSavedRFQProducts();
        }
        void ResetRFQProduct3Type()
        {
            RFQProductsList[2].productType.typeId = 0;
            RFQProductsList[2].productType.typeName = "Other";

            SetNoOfSavedRFQProducts();
        }
        void ResetRFQProduct4Type()
        {
            RFQProductsList[3].productType.typeId = 0;
            RFQProductsList[3].productType.typeName = "Other";

            SetNoOfSavedRFQProducts();
        }
        void ResetRFQProductType(unsigned long productNumber)
        {
            RFQProductsList[productNumber - 1].productType.typeId = 0;
            RFQProductsList[productNumber - 1].productType.typeName = "Other";

            SetNoOfSavedRFQProducts();
        }

        void ResetRFQProduct1Brand()
        {
            RFQProductsList[0].productBrand.brandId = 0;
            RFQProductsList[0].productBrand.brandName = "Other";
        }
        void ResetRFQProduct2Brand()
        {
            RFQProductsList[1].productBrand.brandId = 0;
            RFQProductsList[1].productBrand.brandName = "Other";
        }
        void ResetRFQProduct3Brand()
        {
            RFQProductsList[2].productBrand.brandId = 0;
            RFQProductsList[2].productBrand.brandName = "Other";
        }
        void ResetRFQProduct4Brand()
        {
            RFQProductsList[3].productBrand.brandId = 0;
            RFQProductsList[3].productBrand.brandName = "Other";
        }
        void ResetRFQProductBrand(unsigned long productNumber)
        {
            RFQProductsList[productNumber - 1].productBrand.brandId = 0;
            RFQProductsList[productNumber - 1].productBrand.brandName = "Other";
        }

        void ResetRFQProduct1Model()
        {
            RFQProductsList[0].productModel.modelId = 0;
            RFQProductsList[0].productModel.modelName = "Other";
        }
        void ResetRFQProduct2Model()
        {
            RFQProductsList[1].productModel.modelId = 0;
            RFQProductsList[1].productModel.modelName = "Other";
        }
        void ResetRFQProduct3Model()
        {
            RFQProductsList[2].productModel.modelId = 0;
            RFQProductsList[2].productModel.modelName = "Other";
        }
        void ResetRFQProduct4Model()
        {
            RFQProductsList[3].productModel.modelId = 0;
            RFQProductsList[3].productModel.modelName = "Other";
        }
        void ResetRFQProductModel(unsigned long productNumber)
        {
            RFQProductsList[productNumber - 1].productModel.modelId = 0;
            RFQProductsList[productNumber - 1].productModel.modelName = "Other";
        }

        void ResetRFQProduct1Quantity()
        {
            RFQProductsList[0].productQuantity = 0;

            SetNoOfSavedRFQProducts();
        }
        void ResetRFQProduct2Quantity()
        {
            RFQProductsList[1].productQuantity = 0;

            SetNoOfSavedRFQProducts();
        }
        void ResetRFQProduct3Quantity()
        {
            RFQProductsList[2].productQuantity = 0;

            SetNoOfSavedRFQProducts();
        }
        void ResetRFQProduct4Quantity()
        {
            RFQProductsList[3].productQuantity = 0;

            SetNoOfSavedRFQProducts();
        }
        void ResetRFQProductQuantity(unsigned long productNumber)
        {
            RFQProductsList[productNumber - 1].productQuantity = 0;

            SetNoOfSavedRFQProducts();
        }

        void SetNoOfSavedRFQProducts()
        {
            RFQProductsValid[0] = RFQProductsList[0].productType.typeId + RFQProductsList[0].productQuantity;

            if (RFQProductsValid[0])
                noOfSavedRFQProducts = 1;
            else
                noOfSavedRFQProducts = 0;

            for (int i = 1; i < MAX_RFQ_PRODUCTS; i++)
            {
                RFQProductsValid[i] = RFQProductsValid[i - 1] + RFQProductsList[i].productType.typeId && RFQProductsList[i].productQuantity;

                if (RFQProductsValid[i])
                    noOfSavedRFQProducts = i + 1;
            }
        }
    }*/

}
