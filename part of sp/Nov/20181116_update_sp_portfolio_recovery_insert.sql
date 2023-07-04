
DROP procedure IF EXISTS `sp_portfolio_recovery_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_recovery_insert`()
BEGIN

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);

	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));

	DELETE FROM rpt_portfolio_recovery WHERE year=last_year and month=last_month;

	INSERT INTO rpt_portfolio_recovery(status,lender,year,month,number,amount,createddate)
	select * from (
		SELECT agentjob as status,lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate, '%m') as month,
			COUNT(*) as number,
			SUM(cashamount) as amount,
			curdate() as createddate
		FROM ams_loanaccount
		WHERE agentjob in('Score Collection','JBS Collection','Swift Collection','Express Mercantile','Access Mercantile') and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender,agentjob

		union all

		select 'Field Call' as status,lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate, '%m') as month,
			count(*) as number,
			sum(IF(recoverystatus='Field - Opened' OR recoverystatus='Field - Closed' OR recoverystatus='Field - Placement', cashamount,0)) as amount,
			curdate() as createddate
		FROM ams_loanaccount
		WHERE recoverystatus in('Field - Opened','Field - Closed','Field - Placement') and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender

		union all

		select 'Legal' as status,lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate, '%m') as month,
			count(*) as number,
			sum(IF(recoverystatus='Legal - Placement' OR recoverystatus='Legal - Opened' OR recoverystatus='Legal - Closed',cashamount,0)) as amount,
			curdate() as createddate
		FROM ams_loanaccount
		WHERE recoverystatus in('Legal - Placement','Legal - Opened','Legal - Closed') and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender

		union all

		select 'Company Prepare Stat' as status,lender,
			DATE_FORMAT(settleDate,'%Y') as year,
			DATE_FORMAT(settleDate, '%m') as month,
			count(*) as number,
			sum(IF(recoverystatus='Company - Prepare Stat Demand',cashamount,0)) as amount,
			curdate() as createddate
		FROM ams_loanaccount
		WHERE recoverystatus in('Company - Prepare Stat Demand') and
		YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
		GROUP BY lender
	) as q1
	order by year desc,month desc;
    
    
   

END$$

DELIMITER ;

