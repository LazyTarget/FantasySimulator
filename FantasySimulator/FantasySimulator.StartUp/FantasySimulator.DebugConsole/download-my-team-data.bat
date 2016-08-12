
cd %~dp0

REM :: Enter credentials and entryId (can be retrieved through the /transfers method)
SET entryId=
SET teamName=
SET email=
SET pass=

FantasySimulator.DebugConsole.exe fpl2016-dl /user %email% /pass %pass% /uri "https://fantasy.premierleague.com/drf/transfers" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - Transfers (%teamName%) @{now-utc:yyyyMMdd_HHmmss}.json"
FantasySimulator.DebugConsole.exe fpl2016-dl /user %email% /pass %pass% /uri "https://fantasy.premierleague.com/drf/my-team/%entryId%" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - MyTeam (%teamName%) @{now-utc:yyyyMMdd_HHmmss}.json"
FantasySimulator.DebugConsole.exe fpl2016-dl /user %email% /pass %pass% /uri "https://fantasy.premierleague.com/drf/entry/%entryId%" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - Entry (%teamName%) @{now-utc:yyyyMMdd_HHmmss}.json"
FantasySimulator.DebugConsole.exe fpl2016-dl /user %email% /pass %pass% /uri "https://fantasy.premierleague.com/drf/leagues-entered/%entryId%?renewable=1" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - LeaguesEntered (%teamName%) @{now-utc:yyyyMMdd_HHmmss}.json"
FantasySimulator.DebugConsole.exe fpl2016-dl /user %email% /pass %pass% /uri "https://fantasy.premierleague.com/drf/bootstrap-dynamic" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - Bootstrap-dynamic (%teamName%) @{now-utc:yyyyMMdd_HHmmss}.json"

REM exit