using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using System;
using System.Runtime.Serialization;
using Sitecore.Data.Validators;
using Sitecore.Data.Items;
using Sitecore.Data;
using System.Xml;

namespace Sitecore.Support.Data.Validators.FieldValidators
{
    /// <summary>
    /// Represents a Image Alt Field Validator.
    /// </summary>
    [Serializable]
    public class ImageAltFieldValidator : StandardValidator
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The validator name.</value>
        public override string Name
        {
            get
            {
                return "Image Alternate Text";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Data.Validators.FieldValidators.ImageAltFieldValidator" /> class.
        /// </summary>
        public ImageAltFieldValidator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Data.Validators.FieldValidators.ImageAltFieldValidator" /> class.
        /// </summary>
        /// <param name="info">
        /// The serialization info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public ImageAltFieldValidator(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// When overridden in a derived class, this method contains the code to determine whether the value in the input control is valid.
        /// </summary>
        /// <returns>
        /// The result of the evaluation.
        /// </returns>
        protected override ValidatorResult Evaluate()
        {
            ItemUri itemUri = base.ItemUri;
            if (itemUri == null)
            {
                return ValidatorResult.Valid;
            }
            Field field = base.GetField();
            if (field == null)
            {
                return ValidatorResult.Valid;
            }
            string controlValidationValue = base.ControlValidationValue;
            if (string.IsNullOrEmpty(controlValidationValue))
            {
                return ValidatorResult.Valid;
            }
            if (string.Compare(controlValidationValue, "<image />", StringComparison.InvariantCulture) == 0)
            {
                return ValidatorResult.Valid;
            }

            string attribute = null;
            string attribute2 = null;
            bool isXML = false;
            try
            {
                XmlDocument checkXml = new XmlDocument();
                checkXml.LoadXml(controlValidationValue); // if value is xml
                isXML = true;
            }
            catch
            {
                
            }

            if (isXML)
            {
                XmlValue xmlValue = new XmlValue(controlValidationValue, "image");
                attribute = xmlValue.GetAttribute("mediaid");
                attribute2 = xmlValue.GetAttribute("alt");
            }


            string text = controlValidationValue.StartsWith("/") ? ("/sitecore/media library/" + controlValidationValue.TrimStart(new char[]
            {
                '/'
            })) : string.Empty;
            if (string.IsNullOrEmpty(attribute) && string.IsNullOrEmpty(text))
            {
                return ValidatorResult.Valid;
            }
            

            if (!string.IsNullOrEmpty(attribute2))
            {
                return ValidatorResult.Valid;
            }
            Database database = Factory.GetDatabase(itemUri.DatabaseName);
            Assert.IsNotNull(database, itemUri.DatabaseName);
            MediaItem mediaItem = (!string.IsNullOrEmpty(attribute)) ? database.GetItem(attribute, itemUri.Language) : database.GetItem(text, itemUri.Language);
            if (mediaItem == null)
            {
                return ValidatorResult.Valid;
            }
            string alt = mediaItem.Alt;
            if (!string.IsNullOrEmpty(alt))
            {
                return ValidatorResult.Valid;
            }
            base.Text = base.GetText("Alternate text is missing in the Image field \"{0}\".", new string[]
            {
                field.DisplayName
            });
            return base.GetFailedResult(ValidatorResult.Error);
        }

        /// <summary>
        /// Gets the max validator result.
        /// </summary>
        /// <remarks>
        /// This is used when saving and the validator uses a thread. If the Max Validator Result
        /// is Error or below, the validator does not have to be evaluated before saving.
        /// If the Max Validator Result is CriticalError or FatalError, the validator must have
        /// been evaluated before saving.
        /// </remarks>
        /// <returns>
        /// The max validator result.
        /// </returns>
        protected override ValidatorResult GetMaxValidatorResult()
        {
            return base.GetFailedResult(ValidatorResult.Error);
        }
    }
}
