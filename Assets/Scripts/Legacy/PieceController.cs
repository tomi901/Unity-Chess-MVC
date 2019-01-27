
namespace Chess
{
    public class PieceController
    {

        private readonly Piece model;
        private readonly IPieceView view;

        public PieceController(Piece model, IPieceView view)
        {
            this.model = model;
            this.view = view;

            // View callbacks
            view.OnSetMoveToTile += HandleSetPosition;

            // Model callbacks
            model.OnCoordinatesChanged += (o, e) => SyncPosition();

            // Initialization
            SyncPosition();
            SyncType();
            view.Team = model.Team;
        }


        // Model callbacks

        private void SyncPosition()
        {
            view.Position = model.Coordinates;
        }

        private void SyncType()
        {
            //view.Type = model.Type;
        }


        // View callbacks

        private void HandleSetPosition(object sender, PieceSetTargetMovementArgs e)
        {
            //model.Coordinates = e.moveTo;
        }

    }
}
