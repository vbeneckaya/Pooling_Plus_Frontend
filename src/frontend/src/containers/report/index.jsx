import React, {useEffect} from 'react';
import {useDispatch, useSelector} from 'react-redux';
import ReportPowerBi from 'powerbi-report-component';
import {getReportRequest, reportSelector} from "../../ducks/reports";

const Report = () => {
    const dispatch = useDispatch();

    const report = useSelector(state => reportSelector(state));

    useEffect(() => {
        dispatch(getReportRequest())
    }, []);

    return (
        <div className="report-container">
            <ReportPowerBi embedType="report"
                    tokenType="Embed"
                    accessToken={report.token}
                    embedUrl={report.embedURL}
                    embedId={report.reportID}
                    dashboardId=""
                    pageName=""
                    extraSettings={{
                        filterPaneEnabled: true,
                        navContentPaneEnabled: true,
                    }}
                    permissions="All"
                    style={{
                        height: 'calc(100vh - 32px)',
                        width: '100%',
                        border: '0',
                        background: '#eee'
                    }}
                    onLoad={(report) => {

                    }}
                    onSelectData={(data) => {

                    }}
                    onPageChange={(data) => {

                    }}
                    onTileClicked={data => {

                    }}
            />
        </div>
    );
};

export default Report;
