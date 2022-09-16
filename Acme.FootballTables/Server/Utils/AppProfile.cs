using Acme.FootballTables.Server.Models;
using Acme.FootballTables.Shared;
using AutoMapper;

namespace Acme.FootballTables.Server.Utils
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<Season, SeasonTablesSet>()
                .ForMember(
                    destination => destination.SeasonName,
                    options => options.MapFrom(src => src.Name)
                )
                .ForMember(
                    destination => destination.SeasonId,
                    options => options.MapFrom(src => src.Id)
                )
                .ForMember(
                    destination => destination.Tables,
                    options => options.MapFrom(src => src.LeaguesTables)
                );

            CreateMap<Season, OptionEntry>()
                .ForMember(
                    destination => destination.Value,
                    options => options.MapFrom(src => src.Id)
                )
                .ForMember(
                    destination => destination.Name,
                    options => options.MapFrom(src => src.Name)
                );

            CreateMap<AddSeasonRequest, Season>();

            CreateMap<EditSeasonRequest, Season>();

            CreateMap<Season, GetSeasonResponse>();

            CreateMap<LeagueTable, SeasonTableInfo>();

            CreateMap<LeagueTableEntry, TableEntry>()
                .ForMember(
                    destination => destination.Position,
                    options => options.MapFrom(src => src.Position + "."))
                .ForMember(
                    destination => destination.MatchesPlayed,
                    options => options.MapFrom(src => src.MatchesWon + src.MatchesDrawn + src.MatchesLost))
                .ForMember(
                    destination => destination.GoalDifference,
                    options => options.MapFrom(src => CalculateGoalsDifference(src.GoalsFor, src.GoalsAgainst)));

            CreateMap<AddTableRequest, LeagueTable>()
                .ForMember(
                    destination => destination.AdditionalInfo,
                    options => options.MapFrom(src => ReplaceNullWithEmptyString(src.AdditionalInfo))
                );
        }

        private static string CalculateGoalsDifference(int goalsFor, int goalsAgainst)
        {
            var diff = goalsFor - goalsAgainst;
            if (diff == 0)
            {
                return "0";
            }
            return diff > 0 ? $"+{diff}" : $"{diff}";
        }

        private static string ReplaceNullWithEmptyString(string value)
        {
            return value ?? string.Empty;
        }
    }
}
