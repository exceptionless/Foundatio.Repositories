﻿namespace Foundatio.Repositories {
    public static class SetPagingOptionsExtensions {
        internal const string PageLimitKey = "@PageLimit";
        internal const string PageNumberKey = "@PageNumber";

        public static T UsePaging<T>(this T options, int limit, int? page = null) where T : ICommandOptions {
            options.SetOption(PageLimitKey, limit);
            if (page.HasValue)
                options.SetOption(PageNumberKey, page.Value);

            return options;
        }
    }
}

namespace Foundatio.Repositories.Options {
    public static class ReadPagingOptionsExtensions {
        public static bool ShouldUseLimit<T>(this T options) where T : ICommandOptions {
            return options.HasOption(SetPagingOptionsExtensions.PageLimitKey);
        }

        public static int GetLimit<T>(this T options) where T : ICommandOptions {
            var limit = options.GetOption(SetPagingOptionsExtensions.PageLimitKey, RepositoryConstants.DEFAULT_LIMIT);

            if (limit > RepositoryConstants.MAX_LIMIT)
                return RepositoryConstants.MAX_LIMIT;

            return limit;
        }

        public static bool ShouldUsePage<T>(this T options) where T : ICommandOptions {
            return options.HasOption(SetPagingOptionsExtensions.PageNumberKey);
        }

        public static int GetPage<T>(this T options) where T : ICommandOptions {
            return options.GetOption(SetPagingOptionsExtensions.PageNumberKey, 1);
        }

        public static int GetSkip<T>(this T options) where T : ICommandOptions {
            if (!options.ShouldUseLimit() && !options.ShouldUsePage())
                return 0;

            int limit = options.GetLimit();
            int page = options.GetPage();

            int skip = (page - 1) * limit;
            if (skip < 0)
                skip = 0;

            return skip;
        }
    }
}