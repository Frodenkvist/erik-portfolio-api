using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Model
{
    public class FolderDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<FolderDto> Children { get; set; }
    }
}
