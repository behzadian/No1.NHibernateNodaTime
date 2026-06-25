using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.FaraBank.Api.Repos.Conventions;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.Model;

namespace No1.NHibernateNodaTimeTests.Overrides;

public class Overrides :
	IAutoMappingOverride<AnnualDateEntity>,
	IAutoMappingOverride<DurationEntity>,
	IAutoMappingOverride<InstantCompleteEntity>,
	IAutoMappingOverride<InstantCompactEntity>,
	IAutoMappingOverride<LocalDateEntity>,
	IAutoMappingOverride<LocalDateTimeEntity>,
	IAutoMappingOverride<LocalTimeEntity>,
	IAutoMappingOverride<OffsetDateEntity>,
	IAutoMappingOverride<OffsetDateTimeEntity>,
	IAutoMappingOverride<OffsetEntity>,
	IAutoMappingOverride<OffsetTimeEntity>,
	IAutoMappingOverride<YearMonthEntity>,
	IAutoMappingOverride<ZonedDateTimeEntity>
{
	private readonly Func<string, string> columnNameBuilder = SnakeCaseConventionsConvertor.SnakeCase;

	void IAutoMappingOverride<AnnualDateEntity>.Override(AutoMapping<AnnualDateEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<DurationEntity>.Override(AutoMapping<DurationEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<InstantCompleteEntity>.Override(AutoMapping<InstantCompleteEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<InstantCompactEntity>.Override(AutoMapping<InstantCompactEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<LocalDateEntity>.Override(AutoMapping<LocalDateEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<LocalDateTimeEntity>.Override(AutoMapping<LocalDateTimeEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<LocalTimeEntity>.Override(AutoMapping<LocalTimeEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<OffsetDateEntity>.Override(AutoMapping<OffsetDateEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<OffsetDateTimeEntity>.Override(AutoMapping<OffsetDateTimeEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<OffsetEntity>.Override(AutoMapping<OffsetEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<OffsetTimeEntity>.Override(AutoMapping<OffsetTimeEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<YearMonthEntity>.Override(AutoMapping<YearMonthEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<ZonedDateTimeEntity>.Override(AutoMapping<ZonedDateTimeEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
}