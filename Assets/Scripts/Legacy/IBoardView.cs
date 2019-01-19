using System.Collections.Generic;

namespace Chess
{

    public interface IBoardView
    {

        BoardVector Size { get; set; }

        void HighlightTiles(IEnumerable<BoardVector> tiles);

    }

}