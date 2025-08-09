namespace DMD;

public static class Room_Hooks
{
    public static void ApplyHooks()
    {
        On.RoomSpecificScript.AddRoomSpecificScript += RoomSpecificScript_AddRoomSpecificScript;
    }

    private static void RoomSpecificScript_AddRoomSpecificScript(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
    {
        orig(room);

        var roomName = room.abstractRoom.name;

        // TODO
    }
}
