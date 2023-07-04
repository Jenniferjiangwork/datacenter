
DROP procedure IF EXISTS `sp_portfolio_clientportal_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_clientportal_insert`()
BEGIN

	declare TotalClientLogin int;
	declare Outstandingloan int;

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);

	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));
	set TotalClientLogin = 0;
	set Outstandingloan = 0;

	DELETE FROM rpt_portfolio_clientportal WHERE year=last_year and month=last_month;

	select count(distinct (loginmobile)) into TotalClientLogin from client_loginhistory where YEAR(LoginDate) = last_year and MONTH(LoginDate) = last_month;
	select count(*) into Outstandingloan from ams_loanaccount where (status <> 'Close' AND status <> 'Settled' AND status <> 'Written Off');

	INSERT INTO rpt_portfolio_clientportal(status,year,month,number,createddate)
	SELECT * FROM (
	select 'Total Client Login' as status,
		last_year as year,
		last_month as month,
		TotalClientLogin as number,
		curdate() as createddate

	union all

	select 'Median Login Per Client' as status,
		last_year as year,
		last_month as month,
		ifnull(udf_MedianLoginPerClientByMonth(curdate() - INTERVAL 1 MONTH),0) as number,
		curdate() as createddate

	union all

	select 'Total Client Login/Total Outstanding' as status,
		last_year as year,
		last_month as month,
		CASE WHEN Outstandingloan > 0 THEN round((TotalClientLogin/Outstandingloan)*100) ELSE 0 END as number,
		curdate() as createddate
	) as q1 ORDER BY year desc,month desc;

END$$

DELIMITER ;

