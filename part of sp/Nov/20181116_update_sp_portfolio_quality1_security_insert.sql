
DROP procedure IF EXISTS `sp_portfolio_quality1_security_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_quality1_security_insert`()
BEGIN

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);

	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));

	DELETE FROM rpt_portfolio_quality1_security WHERE year=last_year and month=last_month;

	INSERT INTO rpt_portfolio_quality1_security(status,lender,year,month,number,createddate)
	SELECT * FROM (
		select 
			'Vehicle (Financed Only)' as status,
			lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate,'%m') as month,
			COUNT(*) as number,
			curdate() as createddate
		from ams_loanaccount 
		where loanaccountid in(select loanaccountid from crm_vehicle_loan where vehicleregisterid in(select vehicleregisterid from crm_vehicle_register where financeflag_ai='yes')) and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		group by lender

		union all

		select 
			'Vehicle (Non-Finance)' as status,
			lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate,'%m') as month,
			COUNT(*) as number,
			curdate() as createddate
		from ams_loanaccount 
		where loanaccountid in(select loanaccountid from crm_vehicle_loan where vehicleregisterid in(select vehicleregisterid from crm_vehicle_register where (financeflag_ai='no' or financeflag_ai is null))) and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		group by lender

		union all

		select 
			'Property Only' as status,
			lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate,'%m') as month,
			COUNT(*) as number,
			curdate() as createddate
		from ams_loanaccount 
		where loanaccountid in(select loanaccountid from crm_property_loan where propertyregistid in(select propertyregistid from crm_property_register)) and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		group by lender

		union all

		select 
			'Vehicle & Property' as status,
			lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate,'%m') as month,
			COUNT(*) as number,
			curdate() as createddate
		from ams_loanaccount 
		where loanaccountid in(select loanaccountid from crm_property_loan where propertyregistid in(select propertyregistid from crm_property_register)) and 
		loanaccountid in(select loanaccountid from crm_vehicle_loan where vehicleregisterid in(select vehicleregisterid from crm_vehicle_register)) and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		group by lender
	) as q1
	ORDER BY year desc,month desc;

END$$

DELIMITER ;

