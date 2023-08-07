using ContentProtector.Core.Migrations;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace ContentProtector.Core.Components
{
	public class InitializeMigrationPlan : IComponent
	{
		private readonly ICoreScopeProvider _coreScopeProvider;
		private readonly IMigrationPlanExecutor _migrationPlanExecutor;
		private readonly IKeyValueService _keyValueService;
		private readonly IRuntimeState _runtimeState;

		public InitializeMigrationPlan(ICoreScopeProvider coreScopeProvider, IMigrationPlanExecutor migrationPlanExecutor, IKeyValueService keyValueService, IRuntimeState runtimeState)
		{
			_coreScopeProvider = coreScopeProvider;
			_migrationPlanExecutor = migrationPlanExecutor;
			_keyValueService = keyValueService;
			_runtimeState = runtimeState;
		}

		public void Initialize()
		{
			if (_runtimeState.Level < RuntimeLevel.Run)
			{
				return;
			}

			var migrationPlan = new MigrationPlan("ContentProtector");

			migrationPlan.From(string.Empty)
				.To<CreateTableAndData>("Initialized-ContentProtector");

			var upgrader = new Upgrader(migrationPlan);

			upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
		}

		public void Terminate()
		{
		}
	}
}