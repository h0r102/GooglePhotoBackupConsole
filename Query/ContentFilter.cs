using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.Query
{
	enum ContentCategory
	{
		NONE,
		LANDSCAPES,
		RECEIPTS,
		CITYSCAPES,
		LANDMARKS,
		SELFIES,
		PEOPLE,
		PETS,
		WEDDINGS,
		BIRTHDAYS,
		DOCUMENTS,
		TRAVEL,
		ANIMALS,
		FOOD,
		SPORT,
		NIGHT,
		PERFORMANCES,
		WHITEBOARDS,
		SCREENSHOTS,
		UTILITY,
		ARTS,
		CRAFTS,
		FASHION,
		HOUSES,
		GARDENS,
		FLOWERS,
		HOLIDAYS,
	}
	class ContentFilter : AbstractQuery<ContentFilter>
	{
		public List<ContentCategory> includedContentCategories { get; set; }
		public List<ContentCategory> excludedContentCategories { get; set; }
	}
}
