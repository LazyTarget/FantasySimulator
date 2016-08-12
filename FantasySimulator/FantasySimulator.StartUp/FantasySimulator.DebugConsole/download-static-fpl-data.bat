
cd %~dp0

FantasySimulator.DebugConsole.exe fpl2016-dl /uri "https://fantasy.premierleague.com/drf/bootstrap" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - Bootstrap @{now-utc:yyyyMMdd_HHmmss}.json"
FantasySimulator.DebugConsole.exe fpl2016-dl /uri "https://fantasy.premierleague.com/drf/bootstrap-static" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - Bootstrap-static @{now-utc:yyyyMMdd_HHmmss}.json"

FantasySimulator.DebugConsole.exe fpl2016-dl /uri "https://fantasy.premierleague.com/drf/elements" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - Elements @{now-utc:yyyyMMdd_HHmmss}.json"
FantasySimulator.DebugConsole.exe fpl2016-dl /uri "https://fantasy.premierleague.com/drf/element-types" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - ElementTypes.json"
FantasySimulator.DebugConsole.exe fpl2016-dl /uri "https://fantasy.premierleague.com/drf/region" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - Regions.json"
FantasySimulator.DebugConsole.exe fpl2016-dl /uri "https://fantasy.premierleague.com/drf/teams" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - Teams @{now-utc:yyyyMMdd_HHmmss}.json"
FantasySimulator.DebugConsole.exe fpl2016-dl /uri "https://fantasy.premierleague.com/drf/events" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - Events @{now-utc:yyyyMMdd_HHmmss}.json"
FantasySimulator.DebugConsole.exe fpl2016-dl /uri "https://fantasy.premierleague.com/drf/fixtures" /file "../../App_Data/SoccerSimulator/FPL 2016-17/FPL 2016-17 - Fixtures @{now-utc:yyyyMMdd_HHmmss}.json"

FantasySimulator.DebugConsole.exe fpl2016-dl /uri "http://api.football-data.org/alpha/soccerseasons/426/fixtures" /file "../../App_Data/SoccerSimulator/FPL 2016-17/OpenFootball PL - Fixtures @{now-utc:yyyyMMdd_HHmmss}.json"

REM exit