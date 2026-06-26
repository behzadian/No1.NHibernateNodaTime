using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.FaraBank.Api.Repos.Conventions;
using No1.NHibernateNodaTime;
using No1.NHibernateNodaTimeTests.Model;

namespace No1.NHibernateNodaTimeTests.Overrides;

public class Overrides :
	IAutoMappingOverride<AnnualDateEntity>,
	IAutoMappingOverride<DurationCompactEntity>,
	IAutoMappingOverride<DurationCompleteEntity>,
	IAutoMappingOverride<InstantCompactEntity>,
	IAutoMappingOverride<InstantCompleteEntity>,
	IAutoMappingOverride<LocalDateCompactEntity>,
	IAutoMappingOverride<LocalDateCompleteEntity>,
	IAutoMappingOverride<LocalDateTimeCompactEntity>,
	IAutoMappingOverride<LocalDateTimeCompleteEntity>,
	IAutoMappingOverride<LocalTimeEntity>,
	IAutoMappingOverride<OffsetDateCompactEntity>,
	IAutoMappingOverride<OffsetDateCompleteEntity>,
	IAutoMappingOverride<OffsetDateTimeCompactEntity>,
	IAutoMappingOverride<OffsetDateTimeCompleteEntity>,
	IAutoMappingOverride<OffsetEntity>,
	IAutoMappingOverride<OffsetTimeEntity>,
	IAutoMappingOverride<YearMonthEntity>,
	IAutoMappingOverride<ZonedDateTimeCompactEntity>,
	IAutoMappingOverride<ZonedDateTimeCompleteEntity>
{
	private readonly Func<string, string> columnNameBuilder = SnakeCaseConventionsConvertor.SnakeCase;

	void IAutoMappingOverride<AnnualDateEntity>.Override(AutoMapping<AnnualDateEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<DurationCompactEntity>.Override(AutoMapping<DurationCompactEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<InstantCompleteEntity>.Override(AutoMapping<InstantCompleteEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<InstantCompactEntity>.Override(AutoMapping<InstantCompactEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<LocalDateCompleteEntity>.Override(AutoMapping<LocalDateCompleteEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<LocalDateTimeCompleteEntity>.Override(AutoMapping<LocalDateTimeCompleteEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<LocalTimeEntity>.Override(AutoMapping<LocalTimeEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<OffsetDateCompleteEntity>.Override(AutoMapping<OffsetDateCompleteEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
	void IAutoMappingOverride<OffsetDateTimeCompleteEntity>.Override(AutoMapping<OffsetDateTimeCompleteEntity> mapping) {
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
	void IAutoMappingOverride<ZonedDateTimeCompleteEntity>.Override(AutoMapping<ZonedDateTimeCompleteEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}

	void IAutoMappingOverride<DurationCompleteEntity>.Override(AutoMapping<DurationCompleteEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}

	void IAutoMappingOverride<LocalDateCompactEntity>.Override(AutoMapping<LocalDateCompactEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}

	void IAutoMappingOverride<LocalDateTimeCompactEntity>.Override(AutoMapping<LocalDateTimeCompactEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}

	void IAutoMappingOverride<OffsetDateTimeCompactEntity>.Override(AutoMapping<OffsetDateTimeCompactEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}

	void IAutoMappingOverride<ZonedDateTimeCompactEntity>.Override(AutoMapping<ZonedDateTimeCompactEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}

	void IAutoMappingOverride<OffsetDateCompactEntity>.Override(AutoMapping<OffsetDateCompactEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, columnNameBuilder);
	}
}