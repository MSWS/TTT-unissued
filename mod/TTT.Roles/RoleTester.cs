using CounterStrikeSharp.API.Modules.Cvars;

namespace TTT.Roles;

public class RoleTester {
  private readonly FakeConVar<bool> _traitorTesterBlockTester =
    new("traitor_tester_block_tester",
      "For how long tester button should be disabled after using it. 0 to not block it.");

  private readonly FakeConVar<bool> _traitorTesterBlockTesterMessage =
    new("traitor_tester_block_tester_message",
      "If 1 print to client info about tester cooldown. 0 to disable.");

  private readonly FakeConVar<int> _traitorTesterBlockTesterTime =
    new("traitor_tester_block_tester_time",
      "Determinate for how long tester button should be disabled. 0 to disable tester block.",
      3);

  private readonly FakeConVar<int> _traitorTesterLightDelayTime =
    new("traitor_tester_light_delay_time",
      "Determinate if the lights should be shown right away or after few secodns. 0 to disable.");

  private readonly FakeConVar<int> _traitorTesterLightTime =
    new("traitor_tester_light_time",
      "For how long the lights should be shown. Min value is 3 seconds.", 3);

  private readonly FakeConVar<int> _traitorTesterMaxPlayers =
    new("traitor_tester_max_players",
      "Determinate how many players can be checked at one. 0 to disable it.");
}