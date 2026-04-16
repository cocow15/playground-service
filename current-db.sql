-- DROP SCHEMA dashboard;

CREATE SCHEMA dashboard AUTHORIZATION project;

-- DROP SEQUENCE dashboard.ingest_log_files_id_seq;

CREATE SEQUENCE dashboard.ingest_log_files_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 9223372036854775807
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE dashboard.refresh_tokens_id_seq;

CREATE SEQUENCE dashboard.refresh_tokens_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE dashboard.users_id_seq;

CREATE SEQUENCE dashboard.users_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;-- dashboard."2G_daily" definition

-- Drop table

-- DROP TABLE dashboard."2G_daily";

CREATE TABLE dashboard."2G_daily" (
	bsc text NULL,
	cell_name text NULL,
	date_id text NULL,
	cssr text NULL,
	scr text NULL,
	ra_suc text NULL,
	ra_tot text NULL,
	ra_loa_rej text NULL,
	ra_answpag text NULL,
	ra_service text NULL,
	ra_other text NULL,
	ra_emerg text NULL,
	ra_callree text NULL,
	s_cong text NULL,
	s_dr_c_ntc text NULL,
	s_traf text NULL,
	s_dr_ss text NULL,
	s_dr_bq text NULL,
	s_dr_ta text NULL,
	s_dr_oth text NULL,
	s_av_nr text NULL,
	s_avail text NULL,
	t_as_suc text NULL,
	tch_blocking text NULL,
	t_traf text NULL,
	th_traf text NULL,
	t_dr text NULL,
	new_tdr text NULL,
	t_dr_clm text NULL,
	t_dr_s text NULL,
	t_dr_ss_ul text NULL,
	t_dr_ss_dl text NULL,
	t_dr_ss_bl text NULL,
	t_dr_bq_ul text NULL,
	t_dr_bq_dl text NULL,
	t_dr_bq_bl text NULL,
	t_dr_sud text NULL,
	t_dr_ta text NULL,
	t_dr_oth text NULL,
	t_av_nr text NULL,
	t_avail text NULL,
	t_dwn text NULL,
	hot_suc text NULL,
	hot_rev text NULL,
	hot_lost text NULL,
	ho_suc text NULL,
	ho_rev text NULL,
	ho_lost text NULL,
	hoe_suc text NULL,
	hoe_rev text NULL,
	hoe_lost text NULL,
	hi_suc text NULL,
	hi_rev text NULL,
	hi_lost text NULL,
	ho_per_call text NULL,
	s_mht text NULL,
	t_cmht text NULL,
	cnrocnt text NULL,
	raaccfa text NULL,
	raemcal text NULL,
	racalre text NULL,
	raanpag text NULL,
	raosreq text NULL,
	raother text NULL,
	ratrhfaemcal text NULL,
	ratrhfaother text NULL,
	ratrhfareg text NULL,
	ratrhfaanpag text NULL,
	racalr1 text NULL,
	racalr2 text NULL,
	raapag1 text NULL,
	raapag2 text NULL,
	raapops text NULL,
	raorspe text NULL,
	raordat text NULL,
	csimmass text NULL,
	rejcsimmass text NULL,
	psimmass text NULL,
	rejpsimmass text NULL,
	ccongs text NULL,
	ccalls text NULL,
	cmsestab text NULL,
	cnrelcong text NULL,
	cndrop text NULL,
	cnuchcnt text NULL,
	ctralacc text NULL,
	cnscan text NULL,
	cavaacc text NULL,
	cavascan text NULL,
	cdisss text NULL,
	cdissssub text NULL,
	cdisqa text NULL,
	cdisqasub text NULL,
	cdista text NULL,
	tcassall text NULL,
	tfcassall text NULL,
	tfcassallsub text NULL,
	thcassall text NULL,
	thcassallsub text NULL,
	tassall text NULL,
	tfnrelcong text NULL,
	tfnrelcongsub text NULL,
	thnrelcong text NULL,
	thnrelcongsub text NULL,
	tftralacc text NULL,
	thtralacc text NULL,
	tfnscan text NULL,
	thnscan text NULL,
	tavaacc text NULL,
	tavascan text NULL,
	tnuchcnt text NULL,
	tndrop text NULL,
	tfndrop text NULL,
	tfndropsub text NULL,
	thndrop text NULL,
	thndropsub text NULL,
	tfdissul text NULL,
	tfdissulsub text NULL,
	thdissul text NULL,
	thdissulsub text NULL,
	tfdissdl text NULL,
	tfdissdlsub text NULL,
	thdissdl text NULL,
	thdissdlsub text NULL,
	tfdissbl text NULL,
	tfdissblsub text NULL,
	thdissbl text NULL,
	thdissblsub text NULL,
	tfdisqaul text NULL,
	tfdisqaulsub text NULL,
	thdisqaul text NULL,
	thdisqaulsub text NULL,
	tfdisqadl text NULL,
	tfdisqadlsub text NULL,
	thdisqadl text NULL,
	thdisqadlsub text NULL,
	tfdisqabl text NULL,
	tfdisqablsub text NULL,
	thdisqabl text NULL,
	thdisqablsub text NULL,
	tfsudlos text NULL,
	tfsudlossub text NULL,
	thsudlos text NULL,
	thsudlossub text NULL,
	tfdista text NULL,
	thdista text NULL,
	tdwnacc text NULL,
	tdwnscan text NULL,
	sumohosucc text NULL,
	sumohorev text NULL,
	sumoholost text NULL,
	sumohoatt text NULL,
	sumeohsucc text NULL,
	sumeohrev text NULL,
	sumeohlost text NULL,
	sumeohatt text NULL,
	sumihosucc text NULL,
	sumihorev text NULL,
	sumiholost text NULL,
	sumihoatt text NULL,
	sumiabsucc text NULL,
	sumiawsucc text NULL,
	sumoabsucc text NULL,
	sumoawsucc text NULL,
	tfmsestb text NULL,
	thmsestb text NULL,
	cssr_rs text NULL,
	ich_1 text NULL,
	ich_2 text NULL,
	ich_3 text NULL,
	ich_4 text NULL,
	ich_5 text NULL,
	allpdchacc text NULL,
	allpdchscan text NULL,
	allpdchactacc text NULL,
	allocated_pdch text NULL,
	active_pdch_ratio text NULL,
	pdch_alloc_fail text NULL,
	pchallfail text NULL,
	pchallatt text NULL,
	gprs_throughput_dl text NULL,
	egprs_throughput_dl text NULL,
	gprs_throughput_ul text NULL,
	egprs_throughput_ul text NULL,
	payload_gprs_mbyte text NULL,
	payload_egprs_mbyte text NULL,
	data_payload text NULL,
	tbf_est_suc_ul text NULL,
	tbf_est_suc_dl text NULL,
	tbf_drop_dl text NULL,
	dltbfest text NULL,
	faildltbfest text NULL,
	pschreq text NULL,
	prejtfi text NULL,
	prejoth text NULL,
	ip_transfer_interupt_dl text NULL,
	tbfdlgprs text NULL,
	tbfdlegprs text NULL,
	ldisrr text NULL,
	fludisc text NULL,
	ldistfi text NULL,
	tf_traf_u text NULL,
	th_traf_u text NULL,
	tf_traf_o text NULL,
	th_traf_o text NULL,
	t_dr_ss_ul_u text NULL,
	t_dr_ss_dl_u text NULL,
	t_dr_ss_bl_u text NULL,
	t_dr_bq_ul_u text NULL,
	t_dr_bq_dl_u text NULL,
	t_dr_bq_bl_u text NULL,
	t_dr_sud_u text NULL,
	t_dr_oth_u text NULL,
	t_dr_ss_ul_o text NULL,
	t_dr_ss_dl_o text NULL,
	t_dr_ss_bl_o text NULL,
	t_dr_bq_ul_o text NULL,
	t_dr_bq_dl_o text NULL,
	t_dr_bq_bl_o text NULL,
	t_dr_sud_o text NULL,
	t_dr_oth_o text NULL,
	tbf_completion_rate text NULL,
	disnorm text NULL,
	pdch_congest text NULL,
	sqi_tot_good text NULL,
	sqi_tot_acceptable text NULL,
	sqi_tot_bad text NULL,
	rl_vol_gdl text NULL,
	rl_vol_gul text NULL,
	rl_vol_edl text NULL,
	rl_vol_eul text NULL,
	sdcch_success_rate text NULL,
	cestimmass text NULL,
	preemptpdch text NULL,
	tbf_establishment_sr text NULL,
	num_sdsr text NULL,
	denum_sdsr text NULL,
	num_new_sdsr text NULL,
	denum_nem_sdsr text NULL,
	num_hosr text NULL,
	denum_hosr text NULL,
	num_dcr text NULL,
	denum_dcr text NULL,
	num_new_dcr text NULL,
	denum_new_dcr text NULL,
	num_tbf_dl_success_rate text NULL,
	denum_tbf_dl_success_rate text NULL,
	num_tbf_comp_rate text NULL,
	denum_tbf_comp_rate text NULL,
	num_s_cong text NULL,
	denum_s_cong text NULL,
	num_t_cong text NULL,
	denum_t_cong text NULL,
	num_gprs_throughput_dl text NULL,
	denum_gprs_throughput_dl text NULL,
	num_egprs_throughput_dl text NULL,
	denum_egprs_throughput_dl text NULL,
	num_gprs_throughput_ul text NULL,
	denum_gprs_throughput_ul text NULL,
	num_egprs_throughput_ul text NULL,
	denum_egprs_throughput_ul text NULL,
	dcr text NULL,
	new_dcr text NULL,
	thncedrop text NULL,
	tfncedrop text NULL,
	num_t_avail text NULL,
	denum_t_avail text NULL,
	perlen text NULL,
	preempttbf text NULL,
	movecelltbf text NULL,
	iaulrel text NULL,
	preemptulrel text NULL,
	othulrel text NULL,
	ldisrr_2 text NULL,
	ldisoth text NULL,
	dltbfest_2 text NULL,
	faildltbfest_2 text NULL,
	faildlansw text NULL,
	msestultbf text NULL,
	rxq_050_dl text NULL,
	rxq_050_ul text NULL,
	qual50dl text NULL,
	qual50ul text NULL,
	tbf_dl_establishment_sr_num text NULL,
	tbf_dl_establishment_sr_denum text NULL,
	tbf_ul_est_sr_num text NULL,
	tbf_ul_est_sr_denum text NULL,
	gprs_payload text NULL,
	edge_payload text NULL,
	gprs_throughput_kbps text NULL,
	edge_throughput_kbps text NULL,
	payload_2g_2018 text NULL,
	ich_1_num text NULL,
	ich_1_denum text NULL,
	ich_2_num text NULL,
	ich_2_denum text NULL,
	ich_3_num text NULL,
	ich_3_denum text NULL,
	ich_4_num text NULL,
	ich_4_denum text NULL,
	ich_5_num text NULL,
	ich_5_denum text NULL,
	source_file text NULL,
	ingested_at timestamptz DEFAULT now() NULL
);


-- dashboard."2G_daily_kpi" definition

-- Drop table

-- DROP TABLE dashboard."2G_daily_kpi";

CREATE TABLE dashboard."2G_daily_kpi" (
	"DATETIME" date NULL,
	"SITE_ID" varchar(50) NULL,
	"CELL SUFFIX EID" varchar(50) NULL,
	"BAND" varchar(50) NULL,
	"NE" varchar(50) NULL,
	"SUFFIX" varchar(50) NULL,
	"NEID" varchar(50) NULL,
	"CELL" varchar(50) NULL,
	"SECTOR" varchar(50) NULL,
	"SECTORGROUP" varchar(50) NULL,
	cell_name varchar(50) NULL,
	"AVAIL_NUM" float8 NULL,
	"AVAIL_DENUM" float8 NULL,
	"SDSR_NUM" float8 NULL,
	"SDSR_DENUM" float8 NULL,
	"S_CONG_NUM" float8 NULL,
	"S_CONG_DENUM" float8 NULL,
	"T_CONG_NUM" float8 NULL,
	"T_CONG_DENUM" float8 NULL,
	"TCH_BLOCK" float8 NULL,
	"HOSR_NUM" float8 NULL,
	"HOSR_DENUM" float8 NULL,
	"TCH_DROP" float8 NULL,
	"SDCCH_TRAF" float8 NULL,
	"TCH_TRAF" float8 NULL,
	"PAYLOAD_GPRS_MB" float8 NULL,
	"PAYLOAD_EGPRS_MB" float8 NULL,
	"PAYLOAD_MB" float8 NULL,
	"SDCCH_SR" float8 NULL,
	"TBF_COMP_NUM" float8 NULL,
	"TBF_COMP_DENUM" float8 NULL,
	"TBF_DL_EST_NUM" float8 NULL,
	"TBF_DL_EST_DENUM" float8 NULL,
	"TBF_DL_SR_NUM" float8 NULL,
	"TBF_DL_SR_DENUM" float8 NULL,
	"ICH_1_NUM" float8 NULL,
	"ICH_1_DENUM" float8 NULL,
	"ICH_2_NUM" float8 NULL,
	"ICH_2_DENUM" float8 NULL,
	"ICH_3_NUM" float8 NULL,
	"ICH_3_DENUM" float8 NULL,
	"ICH_4_NUM" float8 NULL,
	"ICH_4_DENUM" float8 NULL,
	"ICH_5_NUM" float8 NULL,
	"ICH_5_DENUM" float8 NULL,
	"TBF_UL_EST_NUM" float8 NULL,
	"TBF_UL_EST_DENUM" float8 NULL,
	ingested_at timestamptz DEFAULT now() NULL
);


-- dashboard."2G_labelling" definition

-- Drop table

-- DROP TABLE dashboard."2G_labelling";

CREATE TABLE dashboard."2G_labelling" (
	"CELL SUFFIX EID" varchar(50) NULL,
	"BAND" varchar(50) NULL,
	"NE" varchar(50) NULL,
	"SUFFIX" varchar(50) NULL
);


-- dashboard.busyhour_kpi definition

-- Drop table

-- DROP TABLE dashboard.busyhour_kpi;

CREATE TABLE dashboard.busyhour_kpi (
	date_id date NULL,
	erbs text NULL,
	eutrancellfdd text NULL,
	cqi float8 NULL,
	num_cqi float8 NULL,
	denum_cqi float8 NULL,
	se float8 NULL,
	num_se float8 NULL,
	denum_se float8 NULL,
	cell_downlink_average_throughput float8 NULL,
	cell_uplink_average_throughput float8 NULL,
	dl_pdcp_user_throughput float8 NULL,
	user_uplink_average_throughput float8 NULL,
	source_file text NULL,
	ingested_at timestamptz DEFAULT now() NULL
);


-- dashboard.ingest_log_files definition

-- Drop table

-- DROP TABLE dashboard.ingest_log_files;

CREATE TABLE dashboard.ingest_log_files (
	id bigserial NOT NULL,
	table_name text NOT NULL,
	source_file text NOT NULL,
	ingested_at timestamptz DEFAULT now() NOT NULL,
	row_count int8 NULL,
	CONSTRAINT ingest_log_files_pkey PRIMARY KEY (id),
	CONSTRAINT ingest_log_files_table_name_source_file_key UNIQUE (table_name, source_file)
);


-- dashboard.lte_cell_daily definition

-- Drop table

-- DROP TABLE dashboard.lte_cell_daily;

CREATE TABLE dashboard.lte_cell_daily (
	area text NULL,
	date_id text NULL,
	erbs text NULL,
	eutrancellfdd text NULL,
	cgi text NULL,
	number_of_rrc_redirections_from_eutrans_to_wcdma text NULL,
	number_of_rrc_redirections_from_eutrans_to_geran text NULL,
	lte_csfb_sr text NULL,
	rrc_setup_success_rate_service text NULL,
	erab_setup_success_rate_all text NULL,
	session_setup_success_rate text NULL,
	session_abnormal_release text NULL,
	intra_frequency_handover_out_success_rate text NULL,
	lte_to_wcdma_redirection_success_rate text NULL,
	lte_to_geran_redirection_success_rate text NULL,
	radio_network_availability_rate text NULL,
	cell_downlink_average_throughput text NULL,
	cell_uplink_average_throughput text NULL,
	dl_pdcp_user_throughput text NULL,
	user_uplink_average_throughput text NULL,
	dl_resource_block_utilizing_rate text NULL,
	ul_resource_block_utilizing_rate text NULL,
	downlink_traffic_volume text NULL,
	uplink_traffic_volume text NULL,
	total_traffic_volume text NULL,
	rrc_connection_release_wcdma text NULL,
	rrc_connection_release_gsm text NULL,
	pmhoprepsucc_utran text NULL,
	pmhoprepatt_utran text NULL,
	pmhoexesucc_utran text NULL,
	pmhoexeatt_utran text NULL,
	pmhoprepsucc_geran text NULL,
	pmhoprepatt_geran text NULL,
	pmhoexesucc_geran text NULL,
	pmhoexeatt_geran text NULL,
	pmhoprepsucclteintraf text NULL,
	pmhoprepsucclteinterf text NULL,
	pmhoprepattlteintraf text NULL,
	pmhoprepattlteinterf text NULL,
	pmhoexesucclteintraf text NULL,
	pmhoexesucclteinterf text NULL,
	pmhoexeattlteintraf text NULL,
	pmhoexeattlteinterf text NULL,
	pmuectxtrelcsfbwcdmaem text NULL,
	pmuectxtestabsucc text NULL,
	pmrrcconnestabsucc text NULL,
	pmrrcconnestabatt text NULL,
	pmrrcconnestabattreatt text NULL,
	pmerabestabsuccinit text NULL,
	pmerabestabattinit text NULL,
	pms1sigconnestabsucc text NULL,
	pms1sigconnestabatt text NULL,
	pmerabrelabnormalenbact text NULL,
	pmerabrelabnormalmmeact text NULL,
	pmerabrelabnormalenb text NULL,
	pmerabrelnormalenb text NULL,
	pmerabrelmme text NULL,
	pmcelldowntimeauto text NULL,
	pmcelldowntimeman text NULL,
	pmpdcpvoldldrb text NULL,
	pmschedactivitycelldl text NULL,
	pmpdcpvoluldrb text NULL,
	pmschedactivitycellul text NULL,
	pmpdcpvoldldrblasttti text NULL,
	pmuethptimedl text NULL,
	pmuethpvolul text NULL,
	pmuethptimeul text NULL,
	pmuectxtrelcsfbwcdma text NULL,
	pmuectxtrelcsfbgsm text NULL,
	pmuectxtrelcsfbgsmem text NULL,
	pmerabrelabnormalenbactuelost text NULL,
	pmerabrelabnormalenbacttnfail text NULL,
	pmerabrelabnormalenbactho text NULL,
	pmerabrelabnormalenbactcdt text NULL,
	pmerabrelabnormalenbacthpr text NULL,
	pmuectxtrelabnormalenbactcdt text NULL,
	pmuectxtrelabnormalenbactho text NULL,
	pmuectxtrelabnormalenbactpe text NULL,
	pmuectxtrelabnormalenbacttnfail text NULL,
	pmuectxtrelabnormalenbactuelost text NULL,
	ul_int_pucch text NULL,
	cu_cell text NULL,
	maximum_user_number_rrcconn text NULL,
	pm_count text NULL,
	pmpagreceived text NULL,
	pmpagdiscarded text NULL,
	pmuectxtrelcsfbwcdma_2 text NULL,
	pmuectxtrelcsfbwcdmaem_2 text NULL,
	pmuectxtrelcsfbgsm_2 text NULL,
	pmuectxtrelcsfbgsmem_2 text NULL,
	max_dl_cell_downlink_throughput text NULL,
	max_ul_cell_downlink_throughput text NULL,
	l_latency_dl_ms text NULL,
	integrity_packetlossrate_dl text NULL,
	integrity_packetlossrate_ul text NULL,
	prepsucclterate text NULL,
	inter_freq_ho text NULL,
	ul_rssi_dbm text NULL,
	se_2 text NULL,
	average_cqi_nonhome text NULL,
	"3g_csfbsr" text NULL,
	volte_traffic_erl text NULL,
	volte_payload_dl text NULL,
	volte_payload_ul text NULL,
	total_traffic_volume_new text NULL,
	ca_payload_gb text NULL,
	prb_max text NULL,
	au_max text NULL,
	source_file text NULL,
	ingested_at timestamptz DEFAULT now() NULL,
	qpsk_num text NULL,
	qpsk_denum text NULL,
	active_user_dl text NULL,
	qpsk_dl text NULL,
	"16qam_dl" text NULL,
	"64qam_dl" text NULL,
	"256qam_dl" text NULL,
	se_new text NULL,
	pmactiveuedlmax text NULL,
	erab_setup_success_rate_all_new text NULL,
	session_setup_success_rate_new text NULL,
	session_abnormal_release_new text NULL,
	dl_resource_block_utilizing_rate_new text NULL,
	ul_resource_block_utilizing_rate_new text NULL,
	downlink_traffic_volume_new text NULL,
	uplink_traffic_volume_new text NULL,
	ul_int_pusch text NULL
);


-- dashboard.lte_day definition

-- Drop table

-- DROP TABLE dashboard.lte_day;

CREATE TABLE dashboard.lte_day (
	"DATETIME" date NULL,
	"SITE_ID" text NULL,
	"NE" text NULL,
	"NEID" text NULL,
	"SUFFIX" text NULL,
	"BAND" text NULL,
	"SECTOR" int4 NULL,
	"SECTORGROUP" int4 NULL,
	"CELLNAME" text NULL,
	"AVAIL" float8 NULL,
	"E-RAB" float8 NULL,
	"RRC" float8 NULL,
	"SSSR" float8 NULL,
	"SAR" float8 NULL,
	"INTRA-FHO" float8 NULL,
	pmhoexeattlteintraf float8 NULL,
	"INTER-FHO" float8 NULL,
	pmhoexeattlteinterf float8 NULL,
	"DL UTIL" float8 NULL,
	"UL UTIL" float8 NULL,
	"PRB_MAX_DL" float8 NULL,
	"PRB_MAX_UL" float8 NULL,
	"PRB_MAX" float8 NULL,
	"AVG CQI" float8 NULL,
	"SE" float8 NULL,
	"USER DL THP" float8 NULL,
	"USER UL THP" float8 NULL,
	"CELL DL THP" float8 NULL,
	"CELL UL THP" float8 NULL,
	"MAX DL THP MBPS" float8 NULL,
	"MAX UL THP MBPS" float8 NULL,
	"LATENCY MS" float8 NULL,
	"UL PUCCH" float8 NULL,
	"UL RSSI DBM" float8 NULL,
	"UL PACKET LOSS" float8 NULL,
	"DL PACKET LOSS" float8 NULL,
	"MAX RRC USER" float8 NULL,
	"MAX ACTIVE USER" float8 NULL,
	"CSFB" float8 NULL,
	"DL VOL MB" float8 NULL,
	"UL VOL MB" float8 NULL,
	"PAYLOAD MB" float8 NULL,
	"VOLTE PAY DL" float8 NULL,
	"VOLTE PAY UL" float8 NULL,
	"PAYLOAD GB" float8 NULL,
	"TRAFFIC ERL" float8 NULL,
	"ACTIVE USER DL" float8 NULL,
	qpsk float8 NULL,
	cqi_busyhour float8 NULL,
	se_busyhour float8 NULL,
	"USER DL THP BUSYHOUR" float8 NULL,
	"USER UL THP BUSYJOUR" float8 NULL,
	"CELL DL THP BUSYHOUR" float8 NULL,
	"CELL UL THP BUSYHOUR" float8 NULL,
	ca_payload_gb float8 NULL,
	"16qam_dl" float8 NULL,
	"64qam_dl" float8 NULL,
	"256qam_dl" float8 NULL
);
CREATE INDEX idx_lte_day_band ON dashboard.lte_day USING btree ("BAND");
CREATE INDEX idx_lte_day_cellname ON dashboard.lte_day USING btree ("CELLNAME");
CREATE INDEX idx_lte_day_datetime ON dashboard.lte_day USING btree ("DATETIME");
CREATE INDEX idx_lte_day_neid ON dashboard.lte_day USING btree ("NEID");
CREATE INDEX idx_lte_day_sectorgroup ON dashboard.lte_day USING btree ("SECTORGROUP");
CREATE INDEX idx_lte_day_sectorgroup_date ON dashboard.lte_day USING btree ("SECTORGROUP", "DATETIME");
CREATE INDEX idx_lte_day_site_band_date ON dashboard.lte_day USING btree ("SITE_ID", "BAND", "DATETIME");
CREATE INDEX idx_lte_day_site_cell_date ON dashboard.lte_day USING btree ("SITE_ID", "CELLNAME", "DATETIME");
CREATE INDEX idx_lte_day_site_id ON dashboard.lte_day USING btree ("SITE_ID");


-- dashboard.packet_loss definition

-- Drop table

-- DROP TABLE dashboard.packet_loss;

CREATE TABLE dashboard.packet_loss (
	date_id text NULL,
	ne_name text NULL,
	packetlossratiofwd text NULL,
	packetlossratiorev text NULL,
	twamp_latency_avg text NULL,
	twamp_pl_avg text NULL,
	twamp_pl_max text NULL,
	sctp_packet_loss text NULL,
	source_file text NULL,
	ingested_at timestamptz DEFAULT now() NULL
);


-- dashboard.packet_loss_hourly definition

-- Drop table

-- DROP TABLE dashboard.packet_loss_hourly;

CREATE TABLE dashboard.packet_loss_hourly (
	date_id text NULL,
	hour_id text NULL,
	ne_name text NULL,
	packetlossratiofwd text NULL,
	packetlossratiorev text NULL,
	twamp_latency_avg text NULL,
	twamp_pl_avg text NULL,
	twamp_pl_max text NULL,
	sctp_packet_loss text NULL,
	source_file text NULL,
	ingested_at timestamptz DEFAULT now() NULL
);


-- dashboard.prb_max definition

-- Drop table

-- DROP TABLE dashboard.prb_max;

CREATE TABLE dashboard.prb_max (
	date_id text NULL,
	erbs text NULL,
	eutrancellfdd text NULL,
	dl_resource_block_utilizing_rate_max text NULL,
	ul_resource_block_utilizing_rate_max text NULL,
	source_file text NULL,
	ingested_at timestamptz DEFAULT now() NULL,
	dl_resource_block_utilizing_rate_max_new text NULL,
	ul_resource_block_utilizing_rate_max_new text NULL
);


-- dashboard.users definition

-- Drop table

-- DROP TABLE dashboard.users;

CREATE TABLE dashboard.users (
	id serial4 NOT NULL,
	username varchar(50) NOT NULL,
	email varchar(255) NOT NULL,
	password_hash text NOT NULL,
	failed_login_attempts int4 DEFAULT 0 NOT NULL,
	locked_until timestamp NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NOT NULL,
	updated_at timestamp DEFAULT CURRENT_TIMESTAMP NOT NULL,
	CONSTRAINT users_email_key UNIQUE (email),
	CONSTRAINT users_pkey PRIMARY KEY (id),
	CONSTRAINT users_username_key UNIQUE (username)
);
CREATE INDEX idx_users_email ON dashboard.users USING btree (email);
CREATE INDEX idx_users_username ON dashboard.users USING btree (username);

-- Table Triggers

create trigger trigger_update_users_updated_at before
update
    on
    dashboard.users for each row execute function dashboard.update_updated_at();


-- dashboard.refresh_tokens definition

-- Drop table

-- DROP TABLE dashboard.refresh_tokens;

CREATE TABLE dashboard.refresh_tokens (
	id serial4 NOT NULL,
	"token" text NOT NULL,
	user_id int4 NOT NULL,
	expires_at timestamp NOT NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NOT NULL,
	revoked_at timestamp NULL,
	CONSTRAINT refresh_tokens_pkey PRIMARY KEY (id),
	CONSTRAINT refresh_tokens_token_key UNIQUE (token),
	CONSTRAINT fk_refresh_tokens_user_id FOREIGN KEY (user_id) REFERENCES dashboard.users(id) ON DELETE CASCADE
);
CREATE INDEX idx_refresh_tokens_token ON dashboard.refresh_tokens USING btree (token);
CREATE INDEX idx_refresh_tokens_user_id ON dashboard.refresh_tokens USING btree (user_id);


-- dashboard.mv_ref_2g_day source

CREATE MATERIALIZED VIEW dashboard.mv_ref_2g_day
TABLESPACE pg_default
AS SELECT DISTINCT "SITE_ID" AS site_id,
    "NEID" AS neid,
    "BAND" AS band,
    cell_name AS cellname
   FROM dashboard."2G_daily_kpi"
  WHERE "SITE_ID" IS NOT NULL AND "NEID" IS NOT NULL AND "BAND" IS NOT NULL AND cell_name IS NOT NULL
WITH DATA;


-- dashboard.mv_ref_lte_day source

CREATE MATERIALIZED VIEW dashboard.mv_ref_lte_day
TABLESPACE pg_default
AS SELECT DISTINCT "SITE_ID" AS site_id,
    "NEID" AS neid,
    "BAND" AS band,
    "CELLNAME" AS cellname
   FROM dashboard.lte_day
  WHERE "SITE_ID" IS NOT NULL AND "NEID" IS NOT NULL AND "BAND" IS NOT NULL AND "CELLNAME" IS NOT NULL
WITH DATA;


-- dashboard.mv_ref_ne_packet_loss source

CREATE MATERIALIZED VIEW dashboard.mv_ref_ne_packet_loss
TABLESPACE pg_default
AS SELECT DISTINCT ne_name
   FROM dashboard.packet_loss
  WHERE ne_name IS NOT NULL
WITH DATA;



-- DROP FUNCTION dashboard.update_updated_at();

CREATE OR REPLACE FUNCTION dashboard.update_updated_at()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$function$
;