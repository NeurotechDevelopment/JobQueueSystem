import { ConfigFileOptions, Modes, EmitModes } from "@odata2ts/odata2ts";

const config: ConfigFileOptions = {
    mode: Modes.service,
    emitMode: EmitModes.ts,
    services: {
        JobsService: {
            source: "src/odata/JobRepositoryOdataMetadata.xml", // local file instead of URL
            output: "src/odata"
        }
    }
};

export default config;