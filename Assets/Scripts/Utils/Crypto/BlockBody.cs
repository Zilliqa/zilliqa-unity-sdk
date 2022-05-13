using System.Collections.Generic;
namespace Zilliqa.Core
{
    public class BlockBody
    {
        public string BlockHash;
        public string HeaderSign;
        public List<MicroBlock> MicroBlockInfos;
    }
}
