
DROP procedure IF EXISTS `sp_portfolio_in_out_insert`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_in_out_insert`()
BEGIN

	DECLARE last_month varchar(2);
	DECLARE last_year varchar(4);

	SET last_month =(select LPAD(MONTH(curdate() - INTERVAL 1 MONTH),2,'0'));
	SET last_year =(select YEAR(curdate() - INTERVAL 1 MONTH));

	DELETE FROM rpt_portfolio_inout WHERE year=last_year and month=last_month;

	INSERT INTO rpt_portfolio_inout(status,lender,year,month,number,amount,createddate)
	SELECT * FROM (
	SELECT
		'1stdrawdown' as status,
		lender,
		DATE_FORMAT(settleDate,'%Y') as year,
		DATE_FORMAT(settleDate,'%m') as month,
		COUNT(*) as number,
		SUM(cashamount) as amount,
		curdate() as createddate
	FROM ams_loanoriginal
	WHERE YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
	GROUP BY lender

	union all
	
	SELECT
		'Subdrawdown' as status,
		lender,
		DATE_FORMAT(settleDate,'%Y') as year,
		DATE_FORMAT(settleDate,'%m') as month,
		COUNT(*) as number,
		SUM(drawdownamount) as amount,
		curdate() as createddate
	FROM ams_loanlinecredit
	WHERE YEAR(settleDate) = last_year and MONTH(settleDate) = last_month
	GROUP BY lender

	union all
	
	SELECT
		'Writtenoff' as status,
		lender,
		(SELECT DATE_FORMAT(writtenoffdate,'%Y') FROM ams_statusdate where loanaccountid=l.loanaccountid) as year,
		(SELECT DATE_FORMAT(writtenoffdate,'%m') FROM ams_statusdate where loanaccountid=l.loanaccountid) as month,
		COUNT(*) as number,
		SUM(cashamount) as amount,
		curdate() as createddate
	FROM ams_loanaccount as l 
	WHERE loanaccountid in(select loanaccountid from ams_statusdate where writtenoffdate is not null and YEAR(writtenoffdate)=last_year and MONTH(writtenoffdate)=last_month)
	GROUP BY lender
	
	union all
	
	SELECT
		'Writtenoffreverse' as status,
		lender,
		(SELECT DATE_FORMAT(writtenoffreversedate,'%Y') FROM ams_statusdate where loanaccountid=l.loanaccountid) as year,
		(SELECT DATE_FORMAT(writtenoffreversedate,'%m') FROM ams_statusdate where loanaccountid=l.loanaccountid) as month,
		COUNT(*) as number,
		SUM(cashamount) as amount,
		curdate() as createddate
	FROM ams_loanaccount as l 
	WHERE loanaccountid in(select loanaccountid from ams_statusdate where writtenoffreversedate is not null and YEAR(writtenoffreversedate)=last_year and MONTH(writtenoffreversedate)=last_month)
	GROUP BY lender
	
	union all
	
	SELECT
		'Discharge' as status,
		lender,
		(SELECT DATE_FORMAT(closedate,'%Y') FROM ams_statusdate where loanaccountid=l.loanaccountid) as year,
		(SELECT DATE_FORMAT(closedate,'%m') FROM ams_statusdate where loanaccountid=l.loanaccountid) as month,
		COUNT(*) as number,
		SUM(cashamount) as amount,
		curdate() as createddate
	FROM ams_loanaccount as l 
	WHERE loanaccountid in(select loanaccountid from ams_statusdate where closedate is not null and YEAR(closedate)=last_year and MONTH(closedate)=last_month)
	GROUP BY lender
	) as q1 ORDER BY year desc,month desc;
END$$

DELIMITER ;

