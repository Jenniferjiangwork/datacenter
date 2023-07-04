
DROP procedure IF EXISTS `sp_portfolio_clientportal`;

DELIMITER $$

CREATE PROCEDURE `sp_portfolio_clientportal`()
BEGIN

	SELECT *,
		CONCAT(year,month) as yearmonth
	FROM rpt_portfolio_clientportal
	ORDER BY year desc,month desc;

END$$

DELIMITER ;

