
DROP procedure IF EXISTS `sp_portfolio_clientportal_column`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_clientportal_column`()
BEGIN

	SELECT distinct
		CONCAT(year,month) as yearmonth
	FROM rpt_portfolio_clientportal
	ORDER BY year desc,month desc;

END$$

DELIMITER ;

