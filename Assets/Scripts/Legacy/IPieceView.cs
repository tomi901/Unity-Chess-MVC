using System;

namespace Chess
{

    public interface IPieceView
    {

        IBoardView Board { get; set; }

        //PieceType Type { get; set; }

        PieceTeam Team { get; set; }

        BoardVector Position { get; set; }

        event EventHandler<PieceSetTargetMovementArgs> OnSetMoveToTile;


    }
}
