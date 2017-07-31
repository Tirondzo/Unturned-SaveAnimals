using SDG.Unturned;
using Steamworks;

namespace SaveAnimals
{
    class CommandSaveWrapper : Command
    {
        public CommandSaveWrapper(Command command)
        {
            this.commandSave = command;

            this._command = command.command;
            this._info = command.info;
            this._help = command.help;
        }


        internal Command commandSave;
        protected override void execute(CSteamID executorID, string parameter)
        {
            AnimalsDataManager.save();
            commandSave.check(executorID, commandSave.command, parameter);
        }
    }
}
