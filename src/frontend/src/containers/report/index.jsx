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
                        /*
                        you can set filters onLoad using:
                        this.report.setFilters([filter]).catch((errors) => {
                          console.log(errors);
                        });*/
                        console.log('Report Loaded!');
                        //this.report = report (Read docs to know how to use report object that is returned)
                    }}
                    onSelectData={(data) => {
                        window.alert('You clicked chart:' + data.visual.title);
                    }}
                    onPageChange={(data) => {
                        console.log('You changed page to:' + data.newPage.displayName);
                    }}
                    onTileClicked={data => {
                        console.log('Data from tile', data);
                    }}
            />
        </div>
    );
};

export default Report;
